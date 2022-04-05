using System.Collections.Generic;
using Baltic.DataModel.Execution;
using Baltic.DataModel.Types;
using Baltic.Types.DataAccess;

namespace Baltic.TaskRegistry.DataAccess
{
    public class TaskBrokerageDaoImpl : ITaskBrokerage
    {

        /// 
        /// <param name="batchUid"></param>
        /// <param name="exec"></param>
        public short AddBatchExecution(string batchUid, BatchExecution exec)
        {
            throw new System.NotImplementedException();
        }

        /// 
        /// <param name="taskUid"></param>
        /// <param name="status"></param>
        public short UpdateTaskStatus(string taskUid, ComputationStatus status)
        {
            throw new System.NotImplementedException();
        }

        public short AddJobExecution(string jobsQueueUid, string jobUid, JobExecution exec)
        {
            throw new System.NotImplementedException();
        }

        public short UpdateBatchExecution(BatchExecution be)
        {
            throw new System.NotImplementedException();
        }

        public short UpdateJobExecution(JobExecution je)
        {
            throw new System.NotImplementedException();
        }

        public TaskExecution GetTaskExecution(string taskUid)
        {
            throw new System.NotImplementedException();
        }

        public BatchExecution GetBatchExecution(string batchMsgUid)
        {
            throw new System.NotImplementedException();
        }

        public JobExecution GetJobExecution(string msgUid)
        {
            throw new System.NotImplementedException();
        }

        public List<JobExecution> GetJobExecutions(string jobUid)
        {
            throw new System.NotImplementedException();
        }

        public short AddJobInstance(string batchMsgUid, string jobUid, JobInstance ji)
        {
            throw new System.NotImplementedException();
        }

        public JobInstance GetJobInstance(string instanceUid)
        {
            throw new System.NotImplementedException();
        }

        public short CloseJobInstance(string instanceUid)
        {
            throw new System.NotImplementedException();
        }

        public ResourceReservationRange GetReservationRange(string batchUid)
        {
            throw new System.NotImplementedException();
        }

        public short SetCurrentExecution(string jobInstanceUid, string jobExecutionUid)
        {
            throw new System.NotImplementedException();
        }

        public List<BatchExecution> GetBatchExecutions(string taskUid)
        {
            throw new System.NotImplementedException();
        }
    }
}