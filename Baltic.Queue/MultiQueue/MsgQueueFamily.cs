using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using Baltic.DataModel.CALExecutable;
using Baltic.DataModel.CALMessages;
using Baltic.Types.QueueAccess;
using Serilog;

namespace Baltic.Queue.MultiQueue {
	public class MsgQueueFamily {

		private List<MsgQueue> _queues;
		private ConcurrentDictionary<string, List<MsgQueueFamily>> _queueSuccessors;
		private List<QueueId> _predecessors;
		private QueueId _id;
		private QueueRemoveMethod _removeMethod;
		private MultiQueue _mq;
		
		private SemaphoreSlim _semaphore;

		public int Count => _queues.Count;

		public bool ToRemove
		{
			get
			{
				if (QueueRemoveMethod.OnDemand == _removeMethod)
					return false;
				foreach(QueueId pred in _predecessors)
					if (-1 <= _mq.CheckQueue(pred)) // queue exists
						return false;
				return true;
			}
		}

		public MsgQueueFamily(QueueId id, QueueRemoveMethod removeMethod, MultiQueue mq){
			_queues = new List<MsgQueue>();
			_queueSuccessors = new ConcurrentDictionary<string, List<MsgQueueFamily>>();
			_predecessors = new List<QueueId>();
			_id = id;
			_removeMethod = removeMethod;
			_mq = mq;
			_semaphore = new SemaphoreSlim(1,1);
		}

		/// 
		/// <param name="id"></param>
		public bool IsCompliant(QueueIdentifiable id)
		{
			// check that the TaskUid and FamilyUid are identical
			if (id.TaskUid != _id.TaskUid || id.FamilyId != _id.FamilyId)
				return false;
			// check if the SeqStack of the "_id" is a sub-list of the SeqStack of the "msg" (compare from the stacks' bottoms)
			int index = id.QueueSeqStack.Count - _id.QueueSeqStack.Count;
			return 0 <= index && id.QueueSeqStack.SubStack(index) == _id.QueueSeqStack;
		}

		public bool IsNamed(QueueId id)
		{
			return id == _id;
		}

		/// 
		/// <param name="msg"></param>
		public short Enqueue(Message msg)
		{
			MsgQueue queue = SelectQueue(msg);
			if (null == queue)
				queue = CreateQueue(msg);
			return queue.Enqueue(msg);
		}

		/// <summary>
		/// Designates the current message as processed (removes it from the queue)
		/// and removes the current (+ successor) queues, if required
		/// </summary>
		/// <param name="msgUid"></param>
		/// <param name="id"></param>
		/// <returns>less than zero if error, else number of removed queues</returns>
		public int Acknowledge(string msgUid, QueueId id){
			MsgQueue queue = SelectQueue(id);
			if (null == queue)
				return -1;
			int ret = queue.RemoveMsg(msgUid);
			if (0 > ret)
				return ret;
			if (ToRemove)
				ret = TryRemoveQueues();
			else if(QueueRemoveMethod.OnEachAcknowledge == _removeMethod && 0 == queue.Count)
				Log.Debug($"MQF: Queue {id} is ready to be deleted.");
			if (0 != ret) // any queue was removed?
				return ret; // return the number of removed queues or error
			
			// if last message from an "OnEachAcknowledge" queue was acknowledged then "pretend" that the queue was removed
			return QueueRemoveMethod.OnEachAcknowledge == _removeMethod && 0 == queue.Count ? 1 : 0;
		}

		/// <summary>
		/// Remove the queue and its successors, depending on its status and set method of removal
		/// </summary>
		/// <param name="queue"></param>
		/// <returns>number of removed queues (current one + queues in successor families)</returns>
		private int TryRemoveQueue(MsgQueue queue)
		{
			if (0 == queue.CountAll)
			{
				string queueSeqStack = queue.SeqStack.ToString();
				Log.Debug("MQF: removing queue: " + _id + queue.ToString());
				_queues.Remove(queue);
				int ret = 0;
				_semaphore.Wait();
				try
				{
					if (_queueSuccessors.ContainsKey(queueSeqStack))
						foreach (MsgQueueFamily successor in _queueSuccessors[queueSeqStack])
							ret += successor.TryRemoveQueues();
					return 1 + ret;
				}
				finally
				{
					_semaphore.Release();
				}
			}
			return 0;
		}

		/// <summary>
		/// If the family can be removed and all the queues in the family are empty, removes all the queues in the family
		/// </summary>
		/// <returns>number of removed queues</returns>
		public int TryRemoveQueues()
		{
			if (!ToRemove)
				return 0;
			int ret = 0;
			// Check that all queues in the family are empty, and do nothing if at least one is not empty
			if (QueueRemoveMethod.OnEachAcknowledge == _removeMethod)
				for (int i = _queues.Count-1; i >= 0 ; i--)
					if (0 != _queues[i].CountAll)
						return 0;
			// Remove all the queues in the family (when all are empty)
			for (int i = _queues.Count-1; i >= 0 ; i--)
				ret += TryRemoveQueue(_queues[i]);
			return ret;
		}

		/// <summary>
		/// Returns the number of tokens in the queue, or indicates that the queue does not exist
		/// </summary>
		/// <param name="id"></param>
		/// <returns></returns>
		public int CheckQueue(QueueId id)
		{
			MsgQueue queue = SelectQueue(id);
			if (null == queue)
				return -2; // the queue does not exist and successor queues could be removed
			if (QueueRemoveMethod.OnEachAcknowledge == _removeMethod && 0 == queue.CountAll)
				return -1; // the queue behaves as if it does not exist, but successor queues should not be removed
			return queue.Count;
		}

		public short RegisterSuccessor(QueueId queueId, MsgQueueFamily successor)
		{
			_semaphore.Wait();
			try
			{
				string queueSeqStack =
					queueId.QueueSeqStack.SubStack(0, queueId.QueueSeqStack.Count - _id.QueueSeqStack.Count).ToString();
				_queueSuccessors.TryAdd(queueSeqStack, new List<MsgQueueFamily>());
				if (_queueSuccessors[queueSeqStack].Contains(successor))
					return -1;
				_queueSuccessors[queueSeqStack].Add(successor);
				return 0;
			}
			finally
			{
				_semaphore.Release();
			}
		}

		/// 
		/// <param name="id"></param>
		private MsgQueue SelectQueue(QueueIdentifiable id)
		{
			SeqTokenStack sts = id.QueueSeqStack.SubStack(0,id.QueueSeqStack.Count - _id.QueueSeqStack.Count);
			return _queues.Find(q => q.IsCompliant(sts));
		}

		public short DistributeTopMessages()
		{
			short ret = 0;
			foreach (MsgQueue queue in _queues)
				if (queue.HasConsumers)
				{
					QueueConsumersHandle consumer = queue.DetermineConsumer();
					if (null == consumer)
						continue;
					short res;
					Message msg = queue.Peek();
					if (null == msg)
						continue;
					if (0 == (res = consumer.Handle.MessageReceived(msg)))
					{
						queue.Allocate(consumer);
						ret += res;
					}
					else
						consumer.LastFailedConnection = DateTime.Now; // TODO adapt to the DB
				}
			return ret;
		}

		public short RegisterConsumer(IQueueConsumer qc, QueueId id)
		{
			MsgQueue queue = SelectQueue(id);
			if (null == queue)
				queue = CreateQueue(id);
			return queue.RegisterConsumer(qc);
		}

		public short RegisterPredecessors(List<QueueId> predecessors)
		{
			foreach(QueueId pred in predecessors)
				if (!_predecessors.Contains(pred))
					_predecessors.Add(pred);
			return 0;
		}

		private MsgQueue CreateQueue(QueueIdentifiable queueId)
		{
			MsgQueue queue =
				new MsgQueue(queueId.QueueSeqStack.SubStack(0, queueId.QueueSeqStack.Count - _id.QueueSeqStack.Count));
			_queues.Add(queue);
			return queue;
		}
	}
}