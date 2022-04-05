using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using Baltic.DataModel.CALExecutable;
using Baltic.DataModel.CALMessages;
using Baltic.Types.QueueAccess;
using Microsoft.Extensions.Configuration;
using Serilog;

namespace Baltic.Queue.MultiQueue {
	public class MultiQueue : IMessageBrokerage {

		/// <summary>
		/// string - TaskUid
		/// </summary>
		private ConcurrentDictionary<string,List<MsgQueueFamily>> _queueFamilies;

		private ConcurrentDictionary<string, SemaphoreSlim> _taskSemaphores;
		private short _logLevel;

		public MultiQueue(MultiQueueDbMock mqdm, IConfiguration configuration){
			_queueFamilies = mqdm._queueFamilies;
			_taskSemaphores = mqdm._taskSemaphores;
			_logLevel = short.Parse(configuration["LogLevel"]);
		}

		/// 
		/// <param name="msg"></param>
		public short Enqueue(Message msg){
			if (!_queueFamilies.ContainsKey(msg.TaskUid))
				return -2;
			LockTaskQueueAccess(msg.TaskUid, 1, "Enqueue");
			try
			{
				MsgQueueFamily mqf = GetCompliantFamily(msg);
				if (null == mqf)
					return -1;
				return mqf.Enqueue(msg);
			}
			finally
			{
				LockTaskQueueAccess(msg.TaskUid, -1, "Enqueue");
			}
		}

		/// <summary>
		/// Checks the current queue status
		/// </summary>
		/// <param name="queue"></param>
		/// <returns>
		/// Number of waiting messages in the queue, or -1 if queue is empty and marked as ready to be removed,
		/// or -2 if the queue does not exist (already removed), or -3 if task Uid is invalid
		/// </returns>
		public int CheckQueue(QueueId queue)
		{
			if (!_queueFamilies.ContainsKey(queue.TaskUid))
				return -3;
			MsgQueueFamily mqf = GetCompliantFamily(queue);
			if (null == mqf)
				return -2;
			return mqf.CheckQueue(queue);
		}

		/// <summary>
		/// Designates the current message as processed (removes it from the queue)
		/// and removes the current (+ successor) queues, if required
		/// </summary>
		/// <param name="msgUid"></param>
		/// <param name="queue"></param>
		/// <returns>less than zero if error, else number of removed queues</returns>
		public int Acknowledge(string msgUid, QueueId queue)
		{
			if (!_queueFamilies.ContainsKey(queue.TaskUid))
				return -3;
			LockTaskQueueAccess(queue.TaskUid, 1, "Acknowledge");
			try
			{
				MsgQueueFamily mqf = GetCompliantFamily(queue);
				if (null == mqf)
					return -1;
				return mqf.Acknowledge(msgUid, queue);
				// TODO - remove queue family with the task
			}
			finally
			{
				LockTaskQueueAccess(queue.TaskUid, -1, "Acknowledge");
			}
		}

		/// <summary>
		/// This is run in a separate thread for each running CTask, and finishes when the CTask is finished
		/// </summary>
		/// <param name="taskUidObject"></param>
		public void Run(Object taskUidObject)
		{
			string taskUid = (string) taskUidObject;
			while (_queueFamilies.ContainsKey(taskUid))
			{
				LockTaskQueueAccess(taskUid, 1, "Run");
				try
				{
					foreach (MsgQueueFamily mqf in _queueFamilies[taskUid])
						mqf.DistributeTopMessages();
				}
				finally
				{
					LockTaskQueueAccess(taskUid, -1, "Run");
				}
				Thread.Sleep(100); // TODO set in parameters
			}
		}

		/// 
		/// <param name="qc"></param>
		/// <param name="id"></param>
		public short RegisterConsumer(IQueueConsumer qc, QueueId id){
			if (!_queueFamilies.ContainsKey(id.TaskUid))
				return -2;
			MsgQueueFamily mqf = GetCompliantFamily(id);
			if (null == mqf)
				return -1;
			return mqf.RegisterConsumer(qc, id);
		}

		public void ClearTask(string taskUid)
		{
			LockTaskQueueAccess(taskUid,1,"ClearTask");
			try
			{
				List<MsgQueueFamily> removed;
				_queueFamilies.TryRemove(taskUid, out removed);
			}
			finally
			{
				LockTaskQueueAccess(taskUid,-1,"ClearTask");
			}
			SemaphoreSlim semaphore;
			_taskSemaphores.TryRemove(taskUid, out semaphore);
		}

		/// <summary>
		/// Adds a new queue family with the given isd, to the list of active queue families
		/// In addition - sets the method in which the queue is to be removed when not needed anymore
		/// </summary>
		/// <param name="id"></param>
		/// <param name="removeMethod"></param>
		/// <returns>0 if ok, less than 0 if error</returns>
		public short AddQueueFamily(QueueId id, QueueRemoveMethod removeMethod){
			if (_queueFamilies.TryAdd(id.TaskUid, new List<MsgQueueFamily>()))
			{
				Thread queueThread = new Thread(Run);
				queueThread.Start(id.TaskUid);
			}
			LockTaskQueueAccess(id.TaskUid, 1, "AddQueueFamily");
			try
			{
				if (_queueFamilies[id.TaskUid].Exists(qf => qf.IsNamed(id)))
					return -1;
				_queueFamilies[id.TaskUid].Add(new MsgQueueFamily(id,removeMethod,this));
				return 0;
			}
			finally
			{
				LockTaskQueueAccess(id.TaskUid, -1, "AddQueueFamily");
			}
		}

		/// <summary>
		/// For the given queue family, registers queues that are its predecessors,
		/// i.e. that supply tokens to the job that feeds this queue family
		/// In addition - sets the given queue family as the successor of these preceding queues
		/// </summary>
		/// <param name="family"></param>
		/// <param name="predecessors"></param>
		/// <returns>0 if ok, less than 0 if error</returns>
		public short RegisterPredecessors(QueueId family, List<QueueId> predecessors){
			if (!_queueFamilies.ContainsKey(family.TaskUid))
				return -2;
			MsgQueueFamily mqf = GetCompliantFamily(family); // effectively, this checks if the 'family' (an ID) is equal to 'mqf''s ID
			if (null == mqf)
				return -1;
			short ret = mqf.RegisterPredecessors(predecessors);
			foreach (QueueId pred in predecessors) // link the current queue family as the successor of the registered preceding queues
			{
				MsgQueueFamily predMqf = GetCompliantFamily(pred);
				if (null != predMqf) // should always be the case, but...
					predMqf.RegisterSuccessor(pred, mqf);
				else
					Log.Error($"MQF: cannot register successor queue family {family} for queue {pred}");
			}
			return ret;
		}

		/// 
		/// <param name="id"></param>
		private MsgQueueFamily GetCompliantFamily(QueueIdentifiable id)
		{
			foreach (MsgQueueFamily mqf in _queueFamilies[id.TaskUid])
				if (mqf.IsCompliant(id))
					return mqf;
			return null;
		}

		/// 
		/// <param name="taskUid"></param>
		/// <param name="lockUnlock"></param>
		/// <param name="label"></param>
		private short LockTaskQueueAccess(string taskUid, short lockUnlock, string label)
		{
			// TODO remove the semaphore from the dictionary when task is finished
			_taskSemaphores.TryAdd(taskUid, new SemaphoreSlim(1,1));
			if (2 < _logLevel)
				Log.Debug(ConsoleString() + "LOCK task (" + taskUid + ") " + 
				          (1 == lockUnlock ? "WAITING" : "RELEASED") + " in: " + label);
			if (0 < lockUnlock)
			{
				_taskSemaphores[taskUid].Wait();
				if (2 < _logLevel)
					Log.Debug(ConsoleString() + "LOCK task (" + taskUid + ") CLOSED in: " + label);
				return 1;
			}
			if (0 > lockUnlock)
			{
				_taskSemaphores[taskUid].Release();
				return 0;
			}
			return -1;
		}
		
		private string ConsoleString(){
			return "## SERVER.MULTIQUEUE ## " + DateTime.Now.ToString("dd.MM.yyyy HH:mm:ss.ffff") + " ## ";
		}
		
	}
}