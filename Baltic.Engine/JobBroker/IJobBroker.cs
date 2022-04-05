using System.Collections.Generic;
using Baltic.DataModel.CALExecutable;
using Baltic.DataModel.CALMessages;
using Baltic.DataModel.Execution;

namespace Baltic.Engine.JobBroker
{
	public interface IJobBroker    
	{
		/// 
		/// <param name="batch"></param>
		/// <param name="bm"></param>
		/// <param name="isNewTokenSet"></param>
		short ActivateJobBatch(CJobBatch batch, BatchInstanceMessage bm, bool isNewTokenSet);

		/// 
		/// <param name="batch"></param>
		/// <param name="jim"></param>
		/// <param name="isNewTokenSet"></param>
		short ActivateJob(CJobBatch batch, JobInstanceMessage jim, bool isNewTokenSet);

		/// 
		/// <param name="batchMsgUid"></param>
		/// <param name="jobQueueIds"></param>
		short ConfirmBatchStart(string batchMsgUid, List<QueueId> jobQueueIds);

		/// 
		/// <param name="instanceUid"></param>
		/// <param name="requiredPinQueues"></param>
		/// <param name="isNewInstance"></param>
		short ConfirmJobStart(string instanceUid, List<QueueId> requiredPinQueues, bool isNewInstance);

		/// 
		/// <param name="status"></param>
		/// <param name="isFailed"></param>
		/// <param name="note"></param>
		short UpdateJobStatus(FullJobStatus status, bool isFailed, string note);

		/// 
		/// <param name="batchMsgUid"></param>
		short UpdateStatusesForBatchInstance(string batchMsgUid);

		/// 
		/// <param name="jobInstanceUid"></param>
		/// <param name="taskUid"></param>
		/// <param name="simpleJob"></param>
		short FinishJobInstance(string jobInstanceUid, string taskUid, bool simpleJob);

		/// 
		/// <param name="jobInstanceUid"></param>
		/// <param name="taskUid"></param>
		short FinishJobExecution(string jobInstanceUid, string taskUid);

		/// 
		/// <param name="taskUid"></param>
		short AbortTask(string taskUid);
	}
}