using System.Collections.Generic;
using Baltic.DataModel.Execution;
using Baltic.DataModel.Types;

namespace Baltic.Types.DataAccess
{
    public interface ITaskBrokerage       
    {
        /// 
        /// <param name="batchUid"></param>
        /// <param name="exec"></param>
        short AddBatchExecution(string batchUid, BatchExecution exec);

        /// 
        /// <param name="taskUid"></param>
        /// <param name="status"></param>
        short UpdateTaskStatus(string taskUid, ComputationStatus status);

		/// 
		/// <param name="batchMsgUid"></param>
		/// <param name="jobUid"></param>
		/// <param name="exec"></param>
		short AddJobExecution(string batchMsgUid, string jobUid, JobExecution exec);

		/// 
		/// <param name="be"></param>
		short UpdateBatchExecution(BatchExecution be);
		/// 
		/// <param name="je"></param>
		short UpdateJobExecution(JobExecution je);

		/// 
		/// <param name="taskUid"></param>
		TaskExecution GetTaskExecution(string taskUid);
		
	    /// 
		/// <param name="jobsQueueUid"></param>
		BatchExecution GetBatchExecution(string jobsQueueUid);

	    /// 
	    /// <param name="msgUid"></param>
	    JobExecution GetJobExecution(string msgUid);

	    /// 
		/// <param name="jobUid"></param>
		List<JobExecution> GetJobExecutions(string jobUid);

	    /// 
		/// <param name="jobsQueueUid"></param>
		/// <param name="jobUid"></param>
		/// <param name="ji"></param>
	    short AddJobInstance(string jobsQueueUid, string jobUid, JobInstance ji);
	    
	    /// 
	    /// <param name="instanceUid"></param>
	    JobInstance GetJobInstance(string instanceUid);
	    
	    /// 
	    /// <param name="instanceUid"></param>
	    short CloseJobInstance(string instanceUid);

	    ResourceReservationRange GetReservationRange(string batchUid);
	    
		/// 
		/// <param name="jobInstanceUid"></param>
		/// <param name="jobExecutionUid"></param>
		short SetCurrentExecution(string jobInstanceUid, string jobExecutionUid);

		/// 
		/// <param name="taskUid"></param>
		List<BatchExecution> GetBatchExecutions(string taskUid);
		
    }
}