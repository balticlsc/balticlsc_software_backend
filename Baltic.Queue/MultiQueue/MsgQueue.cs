using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Threading;
using Baltic.DataModel.CALExecutable;
using Baltic.DataModel.CALMessages;
using Baltic.Types.QueueAccess;

namespace Baltic.Queue.MultiQueue {
	public class MsgQueue {
		public SeqTokenStack SeqStack => _seqStack;

		private int _allocated;
		private List<Message> _messages;
		private List<QueueConsumersHandle> _consumers;
		private SeqTokenStack _seqStack;
		private List<MsgQueueFamily> _successors;

		private SemaphoreSlim _semaphore;

		/// 
		/// <param name="seqStack"></param>
		/// <param name="mq"></param>
		public MsgQueue(SeqTokenStack seqStack)
		{
			_allocated = 0;
			_messages = new List<Message>();
			_consumers = new List<QueueConsumersHandle>();
			_seqStack = seqStack;
			_successors = new List<MsgQueueFamily>();
			_semaphore = new SemaphoreSlim(1,1);
		}

		public int Count => _messages.Count - _allocated;
		public int CountAll => _messages.Count;
		public bool HasConsumers => 0 != _consumers.Count;

		/// 
		/// <param name="sts"></param>
		public bool IsCompliant(SeqTokenStack sts)
		{
			return sts == _seqStack;
		}

		/// 
		/// <param name="msg"></param>
		public short Enqueue(Message msg)
		{
			_semaphore.Wait();
			try
			{
				_messages.Add(msg);
				return 0;
			}
			finally
			{
				_semaphore.Release();
			}
		}

		/// 
		/// <param name="msgUid"></param>
		public short RemoveMsg(string msgUid)
		{
			Message msg = _messages.Find(m => msgUid == m.MsgUid);
			if (null == msg)
				return -1;
			if (_messages.IndexOf(msg) >= _allocated)
				return -2;
			_semaphore.Wait();
			try
			{
				_messages.Remove(msg);
				_allocated--;
				return 0;
			}
			finally
			{
				_semaphore.Release();
			}
		}

		public Message Peek()
		{
			_semaphore.Wait();
			try
			{
				return _messages.Count != _allocated ? _messages[_allocated] : null;
			}
			finally
			{
				_semaphore.Release();
			}
		}

		public short Allocate(QueueConsumersHandle selectedConsumers)
		{
			_semaphore.Wait();
			try
			{
				if (_messages.Count == _allocated)
					return -1;
				_allocated++;

				// for the selectedConsumer, increase the counter
				// if all counters (for responsive consumers) are on their max values - reset all of them to 0
				bool full = true;
				foreach (QueueConsumersHandle qci in _consumers)
					if (qci.Counter < qci.NumberOfConsumerInstances && DateTime.MinValue == qci.LastFailedConnection)
					{
						full = false;
						break;
					}

				if (full)
					foreach (QueueConsumersHandle qci in _consumers)
						qci.Counter = 0;
				selectedConsumers.Counter++;
				return 0;
			}
			finally
			{
				_semaphore.Release();
			}
		}
		
		public QueueConsumersHandle DetermineConsumer()
		{
			_semaphore.Wait();
			try
			{
				QueueConsumersHandle ret = null;
				float minValue = 1;
				foreach (QueueConsumersHandle qci in _consumers)
				{
					float currValue = (float) qci.Counter / qci.NumberOfConsumerInstances;
					if ((currValue < minValue || currValue == minValue && 0 == RandomNumberGenerator.GetInt32(1))
					    && (DateTime.Now - qci.LastFailedConnection).TotalSeconds > 1
					) // TODO - parameter in a config file
					{
						ret = qci;
						minValue = currValue;
					}
				}

				return ret;
			}
			finally
			{
				_semaphore.Release();
			}
		}

		/// 
		/// <param name="qc"></param>
		public short RegisterConsumer(IQueueConsumer qc)
		{
			_semaphore.Wait();
			try
			{
				QueueConsumersHandle qch = _consumers.Find(c => c.Handle == qc);
				if (null == qch)
					_consumers.Add(new QueueConsumersHandle(qc));
				else
					qch.NumberOfConsumerInstances++;
				return 0;
			}
			finally
			{
				_semaphore.Release();
			}
		}

		public override string ToString()
		{
			string ret = "";
			foreach (SeqToken st in _seqStack)
				ret += "." + st.SeqUid + "." + st.No + (st.IsFinal ? "f" : "");
			return ret;
		}
	}
}