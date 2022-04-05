using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Baltic.CommonServices;
using Baltic.DataModel.CALExecutable;
using Baltic.DataModel.CALMessages;
using Baltic.DataModel.Execution;
using Baltic.DataModel.Resources;
using Baltic.DataModel.Types;
using Baltic.Types.DataAccess;
using Baltic.Types.Entities;
using Baltic.Types.QueueAccess;
using Microsoft.Extensions.Configuration;
using Serilog;

namespace Baltic.Engine.JobBroker
{
	public class JobBroker : IJobBroker, IQueueConsumer
	{
		private IMessageBrokerage _queue;
		private INetworkBrokerage _networkRegistry;
		private ITaskBrokerage _taskRegistry;
		private IDataModelImplFactory _factory;
		private IClusterNodeAccessFactory _accessFactory;
		private NodeManager _nodeManager;
		
		/// <summary>
		/// string - jobInstanceUid
		/// </summary>
		private static readonly ConcurrentDictionary<string,SemaphoreSlim> InstanceLocks = new ConcurrentDictionary<string, SemaphoreSlim>();

		private short _logLevel;

		/// 
		/// <param name="q"></param>
		/// <param name="nr"></param>
		/// <param name="tr"></param>
		/// <param name="af"></param>
		/// <param name="dmf"></param>
		/// <param name="nm"></param>
		/// <param name="configuration"></param>
		public JobBroker(IMessageBrokerage q, INetworkBrokerage nr, ITaskBrokerage tr, IClusterNodeAccessFactory af,
			IDataModelImplFactory dmf, NodeManager nm, IConfiguration configuration){
			_queue = q; _networkRegistry = nr; _taskRegistry = tr; _accessFactory = af; _factory = dmf;
			_nodeManager = nm;
			_logLevel = short.Parse(configuration["LogLevel"]);
		}

		/// 
		/// <param name="batch"></param>
		/// <param name="bm"></param>
		/// <param name="isNewTokenSet"></param>
		public short ActivateJobBatch(CJobBatch batch, BatchInstanceMessage bm, bool isNewTokenSet)
		{
			// TODO use "isNewTokenSet"!
			if (!isNewTokenSet)
				return 0;
			
			//*test*
			Log.Debug(ConsoleString() +  " ActivateJobBatch START: " + bm.MsgUid + "\n## " + batch);
			//*test*

			// create and register (as "scheduled") a new batch execution
			BatchExecution be = _factory.CreateBatchExecution();
			be.BatchMsgUid = bm.MsgUid;
			be.SeqStack = bm.JobQueueIds[0].QueueSeqStack.Copy();
			be.Status = ComputationStatus.Scheduled;
			_taskRegistry.AddBatchExecution(batch.Uid, be);

			// create a new queue family for the batch message if needed
			QueueId id = new QueueId(bm);
			bool queueNotExists = 0 == _queue.AddQueueFamily(id,QueueRemoveMethod.OnDemand);

			// create new queue families for all the jobs in this batch
			foreach (QueueId jobQueueId in bm.JobQueueIds)
				_queue.AddQueueFamily(jobQueueId,QueueRemoveMethod.OnDemand);
			
			// push the message to the multi-queue
			short ret = _queue.Enqueue(bm);
			
			// ... and register the JobBroker to receive messages from this queue
			if (queueNotExists)
				_queue.RegisterConsumer(this,id);
			return ret;
		}
		
		/// 
		/// <param name="msg"></param>
		public short MessageReceived(Message msg) // from IQueueConsumer
		{
			if (!(msg is BatchInstanceMessage bm))
				return -2; // should not happen
			
			//*test*
			Log.Debug(ConsoleString() +  " BatchMessage RECEIVED: " + bm.MsgUid);
			//*test*
			
			// TODO - check and use JobBatches in "idle" state (JobBatches waiting to be "reused")

			// retrieve clusters that match resource reservation range for the CJobBatch indicated by the "msg"
			ResourceReservationRange reservationRange = _taskRegistry.GetReservationRange(bm.BatchUid);
			string forcedClusterUid = _taskRegistry.GetTaskExecution(bm.TaskUid).Parameters.ClusterUid;
			CCluster forcedCluster = null;
			if (String.IsNullOrEmpty(forcedClusterUid))
				forcedCluster = _networkRegistry.GetCluster(forcedClusterUid);

			List<CCluster> lc;
			if (null == forcedCluster)
				lc = GetMatchingClusters(reservationRange); // TODO propagate Priority to this point and possibly also the reservationRange
			else
				lc = new List<CCluster>() {forcedCluster};
			
			Dictionary<CCluster, ResourceReservation> availableResources = DetermineActualReservations(lc,reservationRange);
			
			// sort the clusters regarding their suitability for the current "msg"
			List<CCluster> clusters = SortClusters(availableResources);
			
			bool wasTried = false;
			
			// retrieve the respective "scheduled" batch execution 
			BatchExecution be = _taskRegistry.GetBatchExecution(bm.MsgUid);
			foreach (CCluster cl in clusters)
			{
				if (wasTried || null == be) // previous Cluster has rejected the JobBatch - create a new batch execution 
				{
					be = _factory.CreateBatchExecution(); // Note: all rejected instances have the same Uid (JobsQueueUid)
					be.BatchMsgUid = bm.MsgUid;
					be.SeqStack = bm.QueueSeqStack.Copy();
					_taskRegistry.AddBatchExecution(bm.BatchUid, be);
				}
				
				// set remaining execution data for the current batch execution
				be.Cluster = cl;
				be.ActualReservation = availableResources[cl];
				be.Status = ComputationStatus.Idle;
				
				// update the BatchMessage with determined resource's Quota
				bm.Quota = be.ActualReservation;

				// update/add the batch execution to the registry 
				if (wasTried)
					_taskRegistry.AddBatchExecution(bm.BatchUid, be);
				else
					_taskRegistry.UpdateBatchExecution(be);

				Log.Debug($"{ConsoleString()} Trying to submit BatchMessage {bm.MsgUid} to Node {cl.NodeUid}");
				// try to run the Batch Execution on the selected Cluster node
				IQueueConsumer consumer = _accessFactory.CreateQueueConsumerAccess(cl);
				short clusterResponse = consumer.MessageReceived(bm);
				
				if (0 == clusterResponse) // the Cluster has accepted the Batch Execution?
				{  // yes - finish
					Log.Debug($"{ConsoleString()} Node {cl.NodeUid} ACCEPTED BatchMessage {bm.MsgUid}");
					return 0; // JobBatch accepted - finish, otherwise repeat for the next Cluster
				}
				if (-2 != clusterResponse) // the Cluster node has rejected the Batch Execution ?
				{	// no - mark the batch execution as "Rejected"
					be.Status = ComputationStatus.Rejected;
					_taskRegistry.UpdateBatchExecution(be);
					wasTried = true;
					Log.Debug($"{ConsoleString()} Node {cl.NodeUid} REJECTED BatchMessage {bm.MsgUid}");
				}
				else // the Cluster node has not responded or otherwise something has failed
					Log.Debug($"{ConsoleString()} Node {cl.NodeUid} NOT RESPONDED to BatchMessage {bm.MsgUid}");

			}
			
			Log.Debug($"{ConsoleString()} Matching Node was NOT FOUND for BatchMessage {bm.MsgUid}");
			
			return -1;
		}

		private List<CCluster> GetMatchingClusters(ResourceReservationRange reservationRange)
		{
			List<CCluster> clusters = _networkRegistry.GetMatchingClusters(reservationRange);
			// return all clusters matching the reservation range and are also currently registered in the NodeManager (active)
			return clusters.FindAll(c => _nodeManager.ContainsKey(c.NodeUid));
		}

		/// 
		/// <param name="batch"></param>
		/// <param name="jim"></param>
		/// <param name="isNewTokenSet"></param>
		public short ActivateJob(CJobBatch batch, JobInstanceMessage jim, bool isNewTokenSet) // TODO refactor 
		{
			//*test*
			Log.Debug(ConsoleString() + " ActivateJob START: " + jim.MsgUid + "\n## " + jim);
			//*test*

			// TODO - shift this to TaskProcessor (assume that "jim" already has the stack)?
			// Make a simplified SeqStack for the purpose of addressing job messages to be sent to proper job queues
			jim.QueueSeqStack = jim.RequiredPinQueues.Values.ToList()[0].
				QueueSeqStack.Copy(new List<int>(), true, batch.DepthLevel);

			// find the batch execution in which the new job execution should be activated
			BatchExecution be = batch.BatchExecutions.Find(exec => exec.SeqStack == jim.QueueSeqStack);
			if (null == be)
				return -1; // should not happen
			
			// Add a new JobExecution to the TaskRegistry
			JobExecution je = _factory.CreateJobExecution();
			je.JobMsgUid = jim.MsgUid;
			// Set full seq stack for job execution's token queues
			je.SeqStack = jim.RequiredPinQueues.Values.ToList()[0].QueueSeqStack.Copy();
			je.Status = ComputationStatus.Scheduled;
			_taskRegistry.AddJobExecution(be.BatchMsgUid, jim.JobUid, je);

			if (CheckInstanceNecessity(jim,be,isNewTokenSet))
				_queue.Enqueue(jim);
			else if (!jim.IsSimple)
			{
				JobExecutionMessage jem = new JobExecutionMessage()
				{
					BatchUid = jim.BatchUid,
					JobUid = jim.JobUid,
					MsgUid = jim.MsgUid,
					TaskUid = jim.TaskUid,
					QueueSeqStack = jim.QueueSeqStack,
					RequiredPinQueues = jim.RequiredPinQueues
				};
				_queue.Enqueue(jem);
			}

			//*test*
			Log.Debug(ConsoleString() + " Activate Job FINISH: " + jim.MsgUid);
			//*test*
		
			return 0;
		}

		/// 
		/// <param name="batchMsgUid"></param>
		/// <param name="jobQueueIds"></param>
		public short ConfirmBatchStart(string batchMsgUid, List<QueueId> jobQueueIds)
		{
			// retrieve the batch execution based on its Uid
			BatchExecution be = _taskRegistry.GetBatchExecution(batchMsgUid);
			if (null == be)
				return -1;
			// retrieve the consumer (node) that runs this batch execution
			IQueueConsumer consumer = _accessFactory.CreateQueueConsumerAccess(be.Cluster);
			// register the appropriate consumer (node) in all the queues for this batch execution
			foreach(QueueId q in jobQueueIds)
				_queue.RegisterConsumer(consumer,q);
			// register that the batch execution has started working
			be.Status = ComputationStatus.Working;
			be.Start = DateTime.Now;
			_taskRegistry.UpdateBatchExecution(be);
			
			// register that the task has started working
			string taskUid = jobQueueIds.First().TaskUid;
			TaskExecution taskExecution = _taskRegistry.GetTaskExecution(taskUid);
			if (ComputationStatus.Idle == taskExecution.Status)
				_taskRegistry.UpdateTaskStatus(taskUid, ComputationStatus.Working);
			// TODO - handle also other statuses ("Neglected" etc.)
			Log.Debug($"{ConsoleString()} Node {be.Cluster.NodeUid} CONFIRMED completion of BatchMessage {batchMsgUid}");

			return 0;
		}

		/// 
		/// <param name="instanceUid"></param>
		/// <param name="requiredPinQueues"></param>
		/// <param name="isNewInstance"></param>
		public short ConfirmJobStart(string instanceUid, List<QueueId> requiredPinQueues, bool isNewInstance)
		{
			JobInstance ji;
			JobExecution je;

			LockInstanceAccess(instanceUid, 1, "ConfirmJobStart");
			try
			{

				if (isNewInstance) // note: in this case, job execution and job instance have the same Uid
				{
					je = _taskRegistry
						.GetJobExecution(instanceUid); // note: job execution already exists (is "scheduled")
					if (je == null) // this should not happen
						return -1;
					ji = _factory.CreateJobInstance();
					ji.InstanceUid = instanceUid;
					_taskRegistry.AddJobInstance(je.BatchExecution.BatchMsgUid, je.Job.Uid,
						ji); // add job instance to the current CJobBatch and CJob

					// if this is NOT a "simple" job, attach the instance to the execution
					if (!ji.Job.IsSimple)
						je.Instance = ji;
				}
				else
				{
					ji = _taskRegistry.GetJobInstance(instanceUid); // note: job instance already exists
					if (ji == null) // this should not happen
						return -2;
					// find appropriate job execution; when "requiredPinQueues" are not empty (not "simple" job), find the execution compliant with these queues
					// note: job execution already exists (is "scheduled") in the proper CJob
					je = ji.Job.JobExecutions.Find(exec => ComputationStatus.Scheduled == exec.Status &&
					                                       (0 == requiredPinQueues.Count || exec.SeqStack ==
						                                       requiredPinQueues[0].QueueSeqStack));
					if (je == null) // this should never happen
						return -3;

					// attach the instance to the execution
					je.Instance = ji;
					if (ji.Job.IsSimple)
						je.TokensReceived = 1;
				}

				// register the batch's node in all the required pin queues (if any)
				IQueueConsumer consumer = _accessFactory.CreateQueueConsumerAccess(je.BatchExecution?.Cluster);
				foreach (QueueId q in requiredPinQueues)
					_queue.RegisterConsumer(consumer, q);

				// update the status of the job's execution
				if (!isNewInstance || !ji.Job.IsSimple)
				{
					// check that the job is not multitasking or there is no current execution for the instance
					if (!ji.Job.IsMultitasking || null == ji.CurrentExecution)
					{
						// set the obtained JobExecution as the current execution of the JobInstance
						_taskRegistry.SetCurrentExecution(ji.InstanceUid, je.JobMsgUid);
						Log.Debug($"{ConsoleString()} Current JobExecution for instance {ji.InstanceUid} " +
						          $"({je.Job.CallName}) set to " + (string.IsNullOrEmpty(je.JobMsgUid)?"NULL":je.JobMsgUid));
					}

					je.Status = ComputationStatus.Working;
					je.Start = DateTime.Now;
					_taskRegistry.UpdateJobExecution(je);

					// *test*
					Log.Debug(
						$"{ConsoleString()} Activate Job Execution ({je.JobMsgUid}) CONFIRMED for instance: {instanceUid} ({je.Job.CallName})");
					// *test*
				}

				return 0;
			}
			finally
			{
				LockInstanceAccess(instanceUid, -1, "ConfirmJobStart");
			}
		}

		/// 
		/// <param name="status"></param>
		/// <param name="isFailed"></param>
		/// <param name="note"></param>
		public short UpdateJobStatus(FullJobStatus status, bool isFailed, string note)
		{
			short ret = UpdateJobInstanceStatus(status,isFailed,note);
			if (-1 == ret)
				Log.Debug(ConsoleString() + "Error: CurrentExecution for JobInstance " + status.JobInstanceUid + " does not exist - status not changed");
			return ret;
		}

		/// 
		/// <param name="batchMsgUid"></param>
		public short UpdateStatusesForBatchInstance(string batchMsgUid)
		{
			BatchExecution be = _taskRegistry.GetBatchExecution(batchMsgUid);
			if (null == be?.Cluster)
			{
				Log.Debug($"{ConsoleString()} Cluster handle not initialised (UpdateStatusesFrBatchInstance)");
				return -1;
			}
			IBalticNode node = _accessFactory.CreateClusterNodeAccess(be.Cluster);
			List<FullJobStatus> statuses = node.GetBatchJobStatuses(be.BatchMsgUid);
			if (null == statuses)
				return -1;
			short ret = 0;
			foreach (FullJobStatus fjs in statuses)
			{
				short result = UpdateJobInstanceStatus(fjs);
				if (-1 > result)
					ret += (short)(result + 1);
			}
			return (short) (0 == ret ? 0 : ret - 1);
		}

		/// 
		/// <param name="jobInstanceUid"></param>
		/// <param name="taskUid"></param>
		/// <param name="simpleJob"></param>
		public short FinishJobInstance(string jobInstanceUid, string taskUid, bool simpleJob)
		{
			JobInstance ji = _taskRegistry.GetJobInstance(jobInstanceUid);
			if (null == ji) // this should not happen
				return -1;
			LockInstanceAccess(jobInstanceUid, 1, "FinishJobInstance");

			try
			{
				_taskRegistry.CloseJobInstance(jobInstanceUid);

				IBalticNode node = _accessFactory.CreateClusterNodeAccess(ji.BatchExecution.Cluster);
				short ret = node.FinishJobInstance(jobInstanceUid);

				if (simpleJob) // if simple job
				{
					string familyId = ji.Job.Batch.Uid + "." + ji.Job.Uid;
					SeqTokenStack seqStack = ji.Executions[0].SeqStack;
					_queue.Acknowledge(jobInstanceUid, new QueueId(taskUid, familyId, seqStack));
				}
				
				// Check that all JobExecutions and JobInstances are "Completed", and close the BatchExecution if so
				if (!ji.BatchExecution.JobExecutions.Exists(e => DateTime.MinValue == e.Finish) &&
				    !ji.BatchExecution.JobInstances.Exists(i => !i.Completed) && 
				    AllBatchJobsHaveStarted(ji.BatchExecution))
					FinishBatchExecution(ji.BatchExecution, taskUid, node);
				
				Log.Debug(ConsoleString() + " JobInstance FINISHED: " + jobInstanceUid + " (" + ji.Job.CallName + ")");
				return (short) (0 == ret ? 0 : ret - 1);
			}
			finally
			{
				LockInstanceAccess(jobInstanceUid, -1, "FinishJobInstance");
			}
		}

		private bool AllBatchJobsHaveStarted(BatchExecution batchExecution)
		{
			foreach(string jobUid in batchExecution.Batch.Jobs.Select(j => j.Uid))
				if (!batchExecution.JobExecutions.Exists(e => e.Job.Uid == jobUid))
					return false;
			return true;
		}

		private short FinishBatchExecution(BatchExecution execution, string taskUid, IBalticNode node)
		{
			execution.Status = ComputationStatus.Completed;
			execution.Finish = DateTime.Now;
			_taskRegistry.UpdateBatchExecution(execution);
			
			short ret = node.FinishJobBatch(execution.BatchMsgUid);
			Log.Debug(ConsoleString() + " JobBatchExecution FINISHED: " + execution.BatchMsgUid);

			TaskExecution taskExecution = _taskRegistry.GetTaskExecution(taskUid);
			
			// Check that all BatchExecutions within the appropriate Task are finished ("Finish" time is already set)
			if (!taskExecution.Task.Batches.Exists(b => b.BatchExecutions.Exists(
				e => DateTime.MinValue == e.Finish && ComputationStatus.Rejected != e.Status
			)))
			{
				FinishTaskExecution(taskExecution);
			} else
				// Check that no BatchExecution within the appropriate Task is working 
			if (!taskExecution.Task.Batches.Exists(b => b.BatchExecutions.Exists(
				e => DateTime.MinValue != e.Start && DateTime.MinValue == e.Finish && 
				     ComputationStatus.Rejected != e.Status)))
				_taskRegistry.UpdateTaskStatus(taskExecution.Task.Uid,ComputationStatus.Idle);

			return ret;
		}

		private short FinishTaskExecution(TaskExecution taskExecution)
		{
			_taskRegistry.UpdateTaskStatus(taskExecution.Task.Uid,ComputationStatus.Completed);
			_queue.ClearTask(taskExecution.Task.Uid);
			return 0;
		}

		/// 
		/// <param name="jobExecutionUid"></param>
		/// <param name="taskUid"></param>
		public short FinishJobExecution(string jobExecutionUid, string taskUid)
		{
			JobExecution je = _taskRegistry.GetJobExecution(jobExecutionUid);
			if (null == je) // this should not happen
				return -1;

			LockInstanceAccess(je.Instance.InstanceUid, 1, "FinishJobExecution");

			try
			{
				if (ComputationStatus.Idle == je.Status || ComputationStatus.Working == je.Status)
				{
					je.Status = ComputationStatus.Completed;
					je.Finish = DateTime.Now;
					if (0 >= je.Progress)
						je.Progress = 100;
				}

				if (je.Job.IsSimple)
					je.TokensProcessed = 1;

				if (ComputationStatus.Idle == je.Status || ComputationStatus.Working == je.Status || je.Job.IsSimple)
					_taskRegistry.UpdateJobExecution(je);

				JobExecution newJobExecution = null;

				if (je.Job.IsMultitasking)
					newJobExecution = je.Instance.Executions.Find(exec => ComputationStatus.Working == exec.Status);

				// set that there is no current job execution for this instance (if not multitasking)
				_taskRegistry.SetCurrentExecution(je.Instance.InstanceUid, newJobExecution?.JobMsgUid);
				Log.Debug(ConsoleString() + "Current JobExecution for instance " + je.Instance.InstanceUid +
				          " set to " + newJobExecution?.JobMsgUid);

				if (!je.Job.IsSimple) // if not simple job
				{
					string familyId = je.Job.Batch.Uid + "." + je.Job.Uid;
					_queue.Acknowledge(jobExecutionUid, new QueueId(taskUid, familyId, je.SeqStack));
				}

				IBalticNode node = _accessFactory.CreateClusterNodeAccess(je.BatchExecution.Cluster);
				short ret = node.FinishJobExecution(je.Instance.InstanceUid);

				Log.Debug(ConsoleString() + " JobExecution FINISHED: " + jobExecutionUid + " (" + je.Job.CallName +
				          ")");
				return (short) (0 == ret ? 0 : ret - 1);
			}
			finally
			{
				LockInstanceAccess(je.Instance.InstanceUid, -1, "FinishJobExecution");
			}
		}

		public short AbortTask(string taskUid)
		{
			foreach (BatchExecution be in _taskRegistry.GetBatchExecutions(taskUid)
				.FindAll(e => e.Finish == DateTime.MinValue))
			{
				IBalticNode node = _accessFactory.CreateClusterNodeAccess(be.Cluster);
				node.FinishJobBatch(be.BatchMsgUid);
				be.Finish = DateTime.Now;
				be.Status = ComputationStatus.Aborted;
				_taskRegistry.UpdateBatchExecution(be);
				foreach (JobInstance ji in be.JobInstances)
					_taskRegistry.CloseJobInstance(ji.InstanceUid);
				foreach (JobExecution je in be.JobExecutions)
				{
					je.Status = ComputationStatus.Aborted;
					je.Finish = be.Finish;
					_taskRegistry.UpdateJobExecution(je);
				}
			}
			_taskRegistry.UpdateTaskStatus(taskUid,ComputationStatus.Aborted);
			_queue.ClearTask(taskUid);
			return 0;
		}

		/// 
		/// <param name="status"></param>
		/// <param name="isFailed"></param>
		/// <param name="note"></param>
		private short UpdateJobInstanceStatus(FullJobStatus status, bool isFailed = false, string note = null)
		{
			JobInstance ji = _taskRegistry.GetJobInstance(status.JobInstanceUid);
			if (null == ji)
				return -2; // this should not happen
			
			JobExecution je = ji.CurrentExecution;
			if (null == je) // this means that there is no job execution active on the job instance (probably means that the previous job execution is completed and already up-to-date)
				return -1;

			if (null != note)
				je.Note = note;
			if (-1 != status.JobProgress)
				je.Progress = status.JobProgress;
			if (!je.Job.IsSimple)
			{
				if (status.TokensProcessed > je.TokensProcessed)
					je.TokensProcessed = status.TokensProcessed;
				if (status.TokensReceived > je.TokensReceived)
					je.TokensReceived = status.TokensReceived;
			}

			if (isFailed)
				je.Status = ComputationStatus.Failed;
			else if (ComputationStatus.Unknown != status.Status)
				je.Status = status.Status;
			if (isFailed || status.Status == ComputationStatus.Failed)
				je.Finish = DateTime.Now;
			else if (status.TokensReceived == status.TokensProcessed
			) // TODO - check if this can be reported better (is anything in an input queue)
				je.Status = ComputationStatus.Idle;
			else if (ComputationStatus.Unknown == status.Status)
				je.Status = ComputationStatus.Working;
			_taskRegistry.UpdateJobExecution(je);

			return 0;
		}

		/// 
		/// <param name="lc"></param>
		private List<CCluster> SortClusters(Dictionary<CCluster,ResourceReservation> lc){
			// MOCK - sort the list randomly (based on random Guids)
			return lc.Keys.ToList().OrderBy(x => Guid.NewGuid()).ToList();
		}

		/// 
		/// <param name="lc"></param>
		/// <param name="range"></param>
		private Dictionary<CCluster, ResourceReservation> DetermineActualReservations(List<CCluster> lc, ResourceReservationRange range)
		{
			Dictionary<CCluster, ResourceReservation> result = new Dictionary<CCluster, ResourceReservation>();
			foreach (CCluster cluster in lc)
				result.Add(cluster,new ResourceReservation()
				{
					Memory = (range.MaxReservation.Memory + range.MinReservation.Memory) / 2,
					Storage	= (range.MaxReservation.Storage + range.MinReservation.Storage) / 2,
					Cpus = (range.MaxReservation.Cpus + range.MinReservation.Cpus) / 2,
					Gpus = (range.MaxReservation.Gpus + range.MinReservation.Gpus) / 2
				}); // TODO determine the actual reservation according to some algorithm
			return result;
		}

		/// 
		/// <param name="msg"></param>
		/// <param name="be"></param>
		/// <param name="isNewTokenSet"></param>
		private bool CheckInstanceNecessity(JobInstanceMessage msg, BatchExecution be, bool isNewTokenSet)
		{
			if (isNewTokenSet && null != msg.JobUid)
			{
				List<JobInstance> instances = be.JobInstances.FindAll(ji => ji.Job.Uid == msg.JobUid);
				// if no instances of the current job exist - a new instance is necessary
				if (0 == instances.Count)
					return true;
				// TODO - determine if we have enough resources
				return false;
			}
			// TODO determine if we have enough resources for "simple" jobs
			return isNewTokenSet;
		}

		/// 
		/// <param name="jobInstanceUid"></param>
		/// <param name="lockUnlock"></param>
		/// <param name="label"></param>
		private short LockInstanceAccess(string jobInstanceUid, short lockUnlock, string label)
		{
			// TODO remove the semaphore from the dictionary when task is finished
			InstanceLocks.TryAdd(jobInstanceUid, new SemaphoreSlim(1,1));
			if (2 < _logLevel)
				Log.Debug(ConsoleString() + "LOCK job instance (" + jobInstanceUid + ") " + 
				          (1 == lockUnlock ? "WAITING" : "RELEASED") + " in: " + label);
			if (0 < lockUnlock)
			{
				InstanceLocks[jobInstanceUid].Wait();
				if (2 < _logLevel)
					Log.Debug(ConsoleString() + "LOCK job instance (" + jobInstanceUid + ") CLOSED in: " + label);
				return 1;
			}
			if (0 > lockUnlock) {
				InstanceLocks[jobInstanceUid].Release();
				return 0;
			}
			return -1;
		}
	
		private string ConsoleString(){
			return "## SERVER.JOBBROKER ## " + DateTime.Now.ToString("dd.MM.yyyy HH:mm:ss.ffff") + " ## ";
		}
	}
}
