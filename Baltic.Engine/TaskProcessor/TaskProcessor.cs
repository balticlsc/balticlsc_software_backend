using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Baltic.DataModel.CALExecutable;
using Baltic.DataModel.CALMessages;
using Baltic.DataModel.Execution;
using Baltic.DataModel.Types;
using Baltic.Engine.JobBroker;
using Baltic.Types.DataAccess;
using Baltic.Types.QueueAccess;
using Microsoft.Extensions.Configuration;
using Serilog;

namespace Baltic.Engine.TaskProcessor
{
	public class TaskProcessor : ITaskProcessor
	{
		private IMessageProcessing _queue;
		private ITaskProcessing _taskRegistry;
		private IJobBroker _broker;
	
		/// <summary>
		/// string - taskUid
		/// </summary>
		private static readonly ConcurrentDictionary<string,SemaphoreSlim> TaskLocks = new ConcurrentDictionary<string, SemaphoreSlim>();

		private short _logLevel;

		/// 
		/// <param name="q"></param>
		/// <param name="tr"></param>
		/// <param name="b"></param>
		/// <param name="configuration"></param>
		public TaskProcessor(IMessageProcessing q, ITaskProcessing tr, IJobBroker b, IConfiguration configuration)
		{
			_queue = q;
			_taskRegistry = tr;
			_broker = b;
			_logLevel = short.Parse(configuration["LogLevel"]);
		}

		/// 
		/// <param name="tm"></param>
		public short PutTokenMessage(TokenMessage tm)
		{
			// TODO initialise RequiredAccessType in JobMessage-s
			
			QueueId mainQueueName = null;
			short ret = 0;//TODO
			CDataToken currentDataToken = null;
			short batchReady = NotReadyNext;

			//*test*
			Log.Debug(ConsoleString() + "PutTokenMessage START" + "\n$$ " + tm);
			//*test*
    	
			LockTaskAccess(tm.TaskUid, 1, "PutTokenMessage");

			try
			{
				if (!_taskRegistry.IsTaskWorking(tm.TaskUid))
					return -1;
				
				// Start JobBatch - check if all required-strong tokens are available

				CJobBatch currentBatch = _taskRegistry.GetJobBatchForRequiredToken(tm);

				if (null != currentBatch) // this token message matches a required data token for a CJobBatch
				{
					// determine the data token compliant with the current incoming token message
					currentDataToken = currentBatch.GetToken(tm.TokenNo);
					if (null == currentDataToken)
						return -1; // something is wrong (should not happen)

					// determine the token queue id, based on the current token and depths 
					mainQueueName = new QueueId(tm, currentDataToken.Depths, currentBatch.IsSimple, currentBatch.DepthLevel);
					if (_taskRegistry.IsTaskRequiredToken(tm))
						_queue.AddQueueFamily(mainQueueName,QueueRemoveMethod.OnEachAcknowledge);

					batchReady = IsReadyToStart(currentBatch, mainQueueName);

					if (NotReadyNext < batchReady) // the batch is ready to start
					{

						BatchInstanceMessage bm = new BatchInstanceMessage()
						{
							TaskUid = tm.TaskUid,
							BatchUid = currentBatch.Uid,
							// set job queue ids for all the jobs in the batch
							JobQueueIds = currentBatch.Jobs
								.Select(j => new QueueId(mainQueueName, currentBatch, j.Uid)).ToList(),
							DepthLevel = currentBatch.DepthLevel,
							ServiceBuilds = currentBatch.Services.Select(s => s.Build).ToList()
						};

						ret += _broker.ActivateJobBatch(currentBatch, bm, ReadyFirst == batchReady);
					}

				}

				CJob job = _taskRegistry.GetJobForRequiredToken(tm);
				Dictionary<JobInstanceMessage,bool> jobMessageInfos = new Dictionary<JobInstanceMessage, bool>();

				if (null != job)
				{
					// determine the data token compliant with the current incoming token message
					currentDataToken = job.GetToken(tm.TokenNo);
					// If the current token has access credentials (usually it is an external token)
					// update access credentials in a PinsConfig appropriate for the determined Pin
					//if (null != currentDataToken.AccessData)
					//	_taskRegistry.UpdateAccessCredentials(job.Uid, tm.PinName, currentDataToken.AccessData.Values);

					if (null == currentBatch)
					{
						currentBatch = job.Batch;
						mainQueueName = new QueueId(tm, currentDataToken.Depths, job.IsSimple,
							currentBatch.DepthLevel);
					}

					tm.PinName = currentDataToken.PinName;

					// TODO - try to use the batch data token instead of job data token, when needed

					// Move SeqTokens that were not used to generate the queue name to the TokenSeqStack
					tm.TokenSeqStack = tm.QueueSeqStack.Split(currentDataToken.Depths, job.IsSimple,
						currentBatch.DepthLevel);

					// the current token is "internal" for the batch or batch has already started and job should yet be started for the current token
					if ( /*null == batchToStart ||*/ NotReadyFirst < batchReady)
					{
						// Start Job - check if required-strong tokens are available and activate the Job
						short jobReady = IsReadyToStart(job,mainQueueName);
						if (NotReadyNext < jobReady)
						{
							JobInstanceMessage jm = GenerateJobMessage(job,mainQueueName,currentBatch);
							jobMessageInfos.Add(jm,ReadyFirst == jobReady);
						}

						if (ReadyFirst == batchReady)
						{
							// for each job (except for the already processed above) that has its all RequiredStrong tokens being a subset of batch's tokens
							// note: compare just the token numbers
							foreach (CJob overdueJob in currentBatch.Jobs.FindAll(j => job.Uid != j.Uid && !j.Tokens.
								FindAll(t => DataBinding.RequiredStrong == t.Binding).Select(t => t.TokenNo).Except(currentBatch.Tokens.Select(t => t.TokenNo)).Any()))
							{
								jobReady = IsReadyToStart(overdueJob,mainQueueName);
								if (NotReadyNext < jobReady)
								{
									JobInstanceMessage jm = GenerateJobMessage(overdueJob,mainQueueName,currentBatch);
									jobMessageInfos.Add(jm,ReadyFirst == jobReady);
								}
							}
						}
					}
				}

				//*test*
				Log.Debug(ConsoleString() + "PutTokenMessage FINISH: " + tm.MsgUid);
				//*test*

				// Put the message into one of the queues in the MultiQueue, the queue name is generated above
				if (!_taskRegistry.IsTaskProvidedToken(tm))
					ret += _queue.Enqueue(tm);
				// TODO - else if token comes from a job (and not from the frontend), save the "direct" data set being the app output

				foreach(KeyValuePair<JobInstanceMessage,bool> jmi in jobMessageInfos)
					ret += _broker.ActivateJob(currentBatch, jmi.Key, jmi.Value);
				return ret;
			}
			finally
			{
				LockTaskAccess(tm.TaskUid, -1, "PutTokenMessage");
			}
		}

		private const short NotReadyFirst = 0;
		private const short NotReadyNext = 1;
		private const short ReadyFirst = 2;
		private const short ReadyNext = 3;

		/// <summary>
		/// Checks if the given CExecutable (CJob or CJobBatch) is ready for creating a new instance or execution
		/// a) ReadyFirst - the current token message queue does not yet exist and other token message queues (if any) have already at least one token
		/// b) ReadyNext - the current token message queue already exists and other token message queues (if any) are longer
		/// c) NotReadyFirst - the current token message queue does not yet exist but also some other message queue does not exist
		/// d) NotReadyNext - the current token message queue already exists but at least one other token message queue is shorter or equal
		/// </summary>
		/// <param name="elem"></param>
		/// <param name="tokenQueueName"></param>
		/// <returns></returns>
		private short IsReadyToStart(CExecutable elem, QueueId tokenQueueName)
		{
			bool weakToken = false;
			
			foreach (CDataToken dt in elem.Tokens) // check all "RequiredStrong" data tokens other than the current one
			{
				if (dt.Binding == DataBinding.RequiredStrong && dt.TokenNo.ToString() != tokenQueueName.FamilyId) 
				{
					QueueId qname = tokenQueueName.Copy(dt.TokenNo.ToString()); // create a queue id with an updated TokenNo
					if (0 > _queue.CheckQueue(qname))
						return NotReadyFirst; // one of the other queues is not "ready" (no tokens yet)
				}
				else if (dt.TokenNo.ToString() == tokenQueueName.FamilyId && dt.Binding != DataBinding.RequiredStrong)
					weakToken = true;

				// prevent starting a "copy-out"-type job when the appropriate ProvidedExternal token did not yet arrive
				// (the user did not yet provide it); i.e. the "Data" field is not yet set in the "ProvidedExternal" CDataToken
				if (elem is CJob && dt.Binding == DataBinding.ProvidedExternal && null == dt.Data)
					return NotReadyFirst;
			}
			
			// if only the current queue is not "ready" (not exists) - job will be ready for the first time
			if (!weakToken && 0 > _queue.CheckQueue(tokenQueueName))
				return ReadyFirst;

			// for mergers: this token should not start a new execution (next token put onto a "multiple" pin)
			if (elem.IsMerger)
				return NotReadyNext;

			// TODO - merge queues for the same joiner (with many "single" required pins)
			// get number of tokens for the current queue
			long refNumberOfTokens = _queue.CheckQueue(tokenQueueName);
			foreach (CDataToken dt in elem.Tokens) // check all "RequiredStrong" data tokens other than the current one
				if (dt.Binding == DataBinding.RequiredStrong && dt.TokenNo.ToString() != tokenQueueName.FamilyId)
				{
					QueueId qname = tokenQueueName.Copy(dt.TokenNo.ToString()); // create a queue id with an updated TokenNo
					if (refNumberOfTokens >= _queue.CheckQueue(qname))
						return NotReadyNext; // one of the other queues has no more tokens than the current one
				}

			return ReadyNext;
		}

		private JobInstanceMessage GenerateJobMessage(CJob job, QueueId mainQueueName, CJobBatch currentBatch)
		{
			JobInstanceMessage jm = new JobInstanceMessage()
			{
				TaskUid = mainQueueName.TaskUid, JobUid = job.Uid, 
				IsMerger = job.IsMerger, IsSplitter = job.IsSplitter, IsSimple = job.IsSimple,
				BatchUid = currentBatch.Uid, IsMultitasking = job.IsMultitasking,
				// makes a list of all distinct Access Types for non-direct tokens 
				RequiredAccessTypes = job.Tokens.FindAll(t => /*!t.Direct &&*/ null != t.AccessType).
					Select(t=> t.AccessType).Distinct().ToList(),
				Build = job.Build
			};

			QueueId qname;
			// the following loop does the following:
			// 1. creates required pin queue list for required data tokens
			// 2. creates mappings between pin names and token numbers for provided data tokens
			// 3. creates queue families for future queues for provided data tokens
			foreach (CDataToken dt in job.Tokens)
			{
				if (dt.Binding < DataBinding.Provided)
				{
					qname = mainQueueName.Copy(dt.TokenNo
						.ToString()); // create a queue with an updated TokenNo
					jm.RequiredPinQueues.Add(dt.PinName, qname);
				}
			}

			// the following loop creates queue families for the provided data tokens
			// and registers queues for the required data tokens as their predecessors
			foreach (CDataToken dt in job.Tokens)
			{
				if (dt.Binding == DataBinding.Provided)
				{
					CJob nextJob = _taskRegistry.GetJobForRequiredToken(mainQueueName.TaskUid,dt.TokenNo);
					if (null != nextJob)
					{
						CDataToken nextToken = nextJob.Tokens.Find(t => t.TokenNo == dt.TokenNo);
						List<int> adjustedDepths = job.IsSplitter ? AdjustDepths(nextToken.Depths) : nextToken.Depths;
						qname = mainQueueName.Copy(dt.TokenNo
								.ToString(), adjustedDepths, nextJob.IsSimple,
								nextJob.Batch.DepthLevel); // create a queue with an updated TokenNo
					}
					else
						qname = mainQueueName.Copy(dt.TokenNo.ToString()); // TODO - secure for various cases that normally don't happen

					jm.ProvidedPinTokens.Add(dt.PinName, dt.TokenNo);
					_queue.AddQueueFamily(qname,
						null == nextJob || nextJob.IsSimple || CMultiplicity.Multiple == dt.TokenMultiplicity
							? QueueRemoveMethod.PredecessorBased
							: QueueRemoveMethod.OnEachAcknowledge);
					// Register predecessor queues; filter out queues based on required "weak" tokens 
					List<QueueId> requiredStrongQueues = new List<QueueId>();
					foreach (QueueId queue in jm.RequiredPinQueues.Values)
					{
						CDataToken token = job.Tokens.Find(t => t.TokenNo == long.Parse(queue.FamilyId));
						if (null != token && token.Binding != DataBinding.RequiredWeak)
							requiredStrongQueues.Add(queue);
					}
					_queue.RegisterPredecessors(qname, requiredStrongQueues);
				}
			}
			
			Log.Debug($"{ConsoleString()} Generated a Job Message:\n{jm.ToString()}");

			return jm;
		}

		private List<int> AdjustDepths(List<int> nextTokenDepths)
		{
			List<int> ret = new List<int>();
			foreach (int i in nextTokenDepths)
				if (0 != i)
					ret.Add(i-1);
			return ret;
		}

		/// <summary>
		/// Handles message acknowledgement (job done or partially done) by a BatchManager
		/// </summary>
		/// <param name="msgUids"></param>
		/// <param name="jobInstanceUid"></param>
		/// <param name="isFinal"></param>
		/// <param name="isFailed"></param>
		/// <returns> 0 if OK, less than 0 if any Ack fails </returns>
		public short AckMessages(Dictionary<string, QueueId> msgUids, string jobInstanceUid, bool isFinal, bool isFailed)
		{
			// TODO - handle "isFailed"
			if (0 == msgUids.Count)
				return -6; // should not happen
			Log.Debug($"{ConsoleString()} ACK message received for Job Instance: {jobInstanceUid}");
			string taskUid = msgUids.Values.ToList().First().TaskUid;
			LockTaskAccess(taskUid, 1, "AckMessages"); // prevent from accessing this simultaneously for the same CTask

			try
			{
				if (!_taskRegistry.IsTaskWorking(taskUid))
					return -1;
				
				short ret = 0;
				bool anyQueueRemoved = false, successorQueueRemoved = false;
				
				// iterate through all the messages to be acknowledged (queueName, messageUid)
				foreach (KeyValuePair<string, QueueId> msgUid in msgUids)
				{
					// acknowledge (remove) the message in the respective queue
					int queueRemoved = _queue.Acknowledge(msgUid.Key, msgUid.Value);
					short ackResult = 0;
					if (0 < queueRemoved)
					{
						if (!anyQueueRemoved)
							anyQueueRemoved = true;
						if (1 < queueRemoved && !successorQueueRemoved)
							successorQueueRemoved = true;
					}
					else
					{
						if (0 > queueRemoved)
							ackResult = (short) queueRemoved;
						ret = ackResult < ret ? ackResult : ret;
					}
				}

				QueueId queueName = msgUids.Values.ToList().First();
				long tokenNo = long.Parse(queueName.FamilyId);
				CJob job = _taskRegistry.GetJobForRequiredToken(taskUid, tokenNo);
				if (null == job)
					return -4;

				if (isFinal || job.IsSimple || anyQueueRemoved)
				{
					// job reports being finished or any of the queues has been removed?
					if (0 > TryCloseJob(taskUid, jobInstanceUid, job, isFinal, successorQueueRemoved))
					{
						Log.Debug(ConsoleString() + "TryCloseJob failed for JobInstance Uid - " + jobInstanceUid);
						return -5;
					}
				}

				return ret;
			}
			finally
			{
				LockTaskAccess(taskUid, -1, "AckMessages"); // release this to be accessed for the same CTask
			}
		}

		public short AbortTask(string taskUid)
		{
			LockTaskAccess(taskUid, 1, "AbortTask");
			try
			{
				return _broker.AbortTask(taskUid);
			}
			finally
			{
				LockTaskAccess(taskUid, -1, "AbortTask");
			}
		}

		private short TryCloseJob(string taskUid, string jobInstanceUid, CJob job, bool forcedClosure, bool doRecurrence, int recurrenceDepth = 0)
		{
			JobInstance currentJobInstance = job.JobInstances.Find(i => i.InstanceUid == jobInstanceUid);
			if (null == currentJobInstance)
				return -2;
			JobExecution currentJobExecution = currentJobInstance.CurrentExecution;
			Log.Debug($"{ConsoleString()} Attempting to close Job Execution: {currentJobExecution?.JobMsgUid} " +
			          $"for instance {jobInstanceUid}, at recurrence depth: {recurrenceDepth}");
			if (null != currentJobExecution)
			{
				// check incoming queues when needed (done always for recurrence levels other than the top one) 
				if (0 < recurrenceDepth || !forcedClosure && !job.IsSimple)
					// check all the "incoming" queues
					foreach (CDataToken token in job.Tokens)
						if (DataBinding.RequiredStrong == token.Binding)
						{
							QueueId updatedQueueName = new QueueId(taskUid, token.TokenNo.ToString(),
								currentJobExecution.SeqStack);
							if (0 <= _queue.CheckQueue(updatedQueueName))
								return 0; // if any of the queues exist - do not finish the JobExecution
						}

				_broker.FinishJobExecution(currentJobExecution.JobMsgUid, taskUid);
			}

			if (doRecurrence)
				foreach (CDataToken token in job.Tokens.FindAll(tok => DataBinding.Provided == tok.Binding))
				{
					CJob nextJob = _taskRegistry.GetJobForRequiredToken(taskUid,token.TokenNo);
					if (null != nextJob) // should always be the case (in the final version)
						foreach (JobInstance instance in nextJob.JobInstances.FindAll(inst => !inst.Completed && 0 != inst.Executions.Count))
							TryCloseJob(taskUid, instance.InstanceUid, nextJob, false, true, recurrenceDepth+1);
				}

			SeqTokenStack currentInstanceStack = null != currentJobExecution
				? currentJobExecution.SeqStack
				: currentJobInstance.Executions.First().SeqStack;
			// Check that there is no JobExecution for this Job that is not finished, and it is meant to be run on
			// the same JobInstance (their SeqStacks match)
			// (job.CallName == "" || job.CallName == "") && job.JobExecutions.Count >= 10
			if (!job.JobExecutions.Exists(je =>
				DateTime.MinValue == je.Finish && je.SeqStack == currentInstanceStack))
			{
				if (forcedClosure || job.IsSimple || (!job.IsSimple && !job.IsMerger))
					// check all the "incoming" queues
					foreach (CDataToken token in job.Tokens)
						if (DataBinding.RequiredStrong == token.Binding)
						{
							QueueId updatedQueueName = new QueueId(taskUid, token.TokenNo.ToString(),
								currentInstanceStack);
							if (-1 <= _queue.CheckQueue(updatedQueueName))
								return 0; // if any of the queues exist - do not finish the JobInstance
						}

				_broker.FinishJobInstance(jobInstanceUid, taskUid, job.IsSimple); // close the job instance
			}

			return 0;
		}

		/// 
		/// <param name="taskUid"></param>
		/// <param name="lockUnlock"></param>
		/// <param name="label"></param>
		private short LockTaskAccess(string taskUid, short lockUnlock, string label)
		{
			// TODO remove the semaphore from the dictionary when task is finished
			TaskLocks.TryAdd(taskUid, new SemaphoreSlim(1,1));
			if (2 < _logLevel)
				Log.Debug(ConsoleString() + "LOCK task (" + taskUid + ") " + 
				          (1 == lockUnlock ? "WAITING" : "RELEASED") + " in: " + label);
			if (0 < lockUnlock)
			{
				TaskLocks[taskUid].Wait();
				if (2 < _logLevel)
					Log.Debug(ConsoleString() + "LOCK task (" + taskUid + ") CLOSED in: " + label);
				return 1;
			}
			if (0 > lockUnlock) {
				TaskLocks[taskUid].Release();
				return 0;
			}
			return -1;
		}

		private string ConsoleString()
		{
			return "## SERVER.TASKPROC ## " + DateTime.Now.ToString("dd.MM.yyyy HH:mm:ss.ffff") + " ## ";
		}

	}
}
