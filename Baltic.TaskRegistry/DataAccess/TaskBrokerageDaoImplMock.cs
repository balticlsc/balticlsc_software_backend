using System;
using System.Collections.Generic;
using Baltic.DataModel.CALExecutable;
using Baltic.DataModel.Execution;
using Baltic.DataModel.Types;
using Baltic.Types.DataAccess;

namespace Baltic.TaskRegistry.DataAccess
{
    public class TaskBrokerageDaoImplMock : ITaskBrokerage
    {
        private List<CTask> _storedTasks;

        public TaskBrokerageDaoImplMock(TaskRegistryMock taskRegistry)
        {
            _storedTasks = taskRegistry.StoredTasks;
        }

        /// 
        /// <param name="batchUid"></param>
        /// <param name="exec"></param>
        public short AddBatchExecution(string batchUid, BatchExecution exec)
        {
            CJobBatch batch;
            foreach(CTask task in _storedTasks)
                if (null != (batch = task.Batches.Find(b => b.Uid == batchUid)))
                {
                    batch.BatchExecutions.Add(exec);
                    exec.Batch = batch;
                    return 0;
                }
            return -1;
        }

        /// 
        /// <param name="taskUid"></param>
        /// <param name="status"></param>
        public short UpdateTaskStatus(string taskUid, ComputationStatus status)
        {
            CTask task = _storedTasks.Find(t => t.Uid == taskUid);
            if (null == task)
                return -1;
            task.Execution.Status = status;
            if (status == ComputationStatus.Completed || status == ComputationStatus.Aborted)
                task.Execution.Finish = DateTime.Now;
            return 0;
        }

        public short AddJobExecution(string batchMsgUid, string jobUid, JobExecution exec)
        {
            BatchExecution be;
            foreach(CTask task in _storedTasks)
                foreach(CJobBatch batch in task.Batches)
                    if (null != (be = batch.BatchExecutions.Find(bi => batchMsgUid == bi.BatchMsgUid)))
                    {
                        be.JobExecutions.Add(exec);
                        exec.BatchExecution = be;
                        if (null != jobUid) // this is a "normal" and not a "system" job instance
                        {
                            CJob job = batch.Jobs.Find(j => j.Uid == jobUid);
                            if (null != job)
                            {
                                job.JobExecutions.Add(exec);
                                exec.Job = job;
                            }
                            else // this should never happen
                            {
                                be.JobExecutions.Remove(exec);
                                return -2;
                            }
                        }
                        return 0;
                    }
            return -1;
        }
        
        public short UpdateBatchExecution(BatchExecution be)
        {
            return 0; // does nothing, because this is a mock, and 'be' is already "in the database"
        }

        public short UpdateJobExecution(JobExecution je)
        {
            JobInstance ji = je.Instance;
            if (null != ji && !ji.Executions.Contains(je))
                ji.Executions.Add(je); // add this job execution to the instance
            return 0; // does nothing more, because this is a mock, and 'je' is already "in the database"
        }

        public TaskExecution GetTaskExecution(string taskUid)
        {
            return _storedTasks.Find(t => t.Uid == taskUid)?.Execution;
        }

        public BatchExecution GetBatchExecution(string batchMsgUid)
        {
            foreach (CTask task in _storedTasks)
            foreach (CJobBatch batch in task.Batches)
            {
                BatchExecution be = batch.BatchExecutions.Find(ex => ex.BatchMsgUid == batchMsgUid
                                                                    && ComputationStatus.Rejected != ex.Status);
                if (null != be)
                    return be;
            }
            return null;
        }

        public JobExecution GetJobExecution(string msgUid)
        {
            foreach (CTask task in _storedTasks)
            foreach (CJobBatch batch in task.Batches)
            foreach (BatchExecution be in batch.BatchExecutions)
            {
                JobExecution je = be.JobExecutions.Find(ex => ex.JobMsgUid == msgUid);
                if (null != je)
                    return je;
            }

            return null;
        }

        public List<JobExecution> GetJobExecutions(string jobUid)
        {
            foreach (CTask task in _storedTasks)
            foreach (CJobBatch batch in task.Batches)
            {
                CJob job = batch.Jobs.Find(j => j.Uid == jobUid);
                if (null != job)
                    return job.JobExecutions;
            }

            return null;
        }

        public short AddJobInstance(string batchMsgUid, string jobUid, JobInstance ji)
        {
            BatchExecution be = GetBatchExecution(batchMsgUid);
            CJob job = be.Batch.Jobs.Find(j => j.Uid == jobUid);
            if (null == job)
                return -1;
            be.JobInstances.Add(ji);
            ji.BatchExecution = be;
            job.JobInstances.Add(ji);
            ji.Job = job;
            return 0;
        }

        public JobInstance GetJobInstance(string instanceUid)
        {
            foreach(CTask task in _storedTasks)
                foreach (CJobBatch batch in task.Batches)
                foreach (BatchExecution be in batch.BatchExecutions)
                {
                    JobInstance ji = be.JobInstances.Find(i => i.InstanceUid == instanceUid);
                    if (null != ji)
                        return ji;
                }
            return null;
        }

        public short CloseJobInstance(string instanceUid)
        {
            GetJobInstance(instanceUid).Completed = true;
            return 0;
        }

        public ResourceReservationRange GetReservationRange(string batchUid)
        {
            foreach (var task in _storedTasks)
                foreach (var batch in task.Batches)
                    if (batchUid == batch.Uid)
                        return batch.DerivedReservationRange;
            return null;
        }

        public short SetCurrentExecution(string jobInstanceUid, string jobExecutionUid)
        {
            JobInstance ji = GetJobInstance(jobInstanceUid);
            if (null == ji)
                return -1; // this should not happen
            JobExecution je = null;
            if (null != jobExecutionUid)
                je = GetJobExecution(jobExecutionUid);
            ji.CurrentExecution = je;
            return 0;
        }

        public List<BatchExecution> GetBatchExecutions(string taskUid)
        {
            List<BatchExecution> ret = new List<BatchExecution>();
            CTask task = _storedTasks.Find(t => t.Uid == taskUid);
            if (null != task)
                foreach (CJobBatch batch in task.Batches)
                    ret.AddRange(batch.BatchExecutions);
            return ret;
        }
    }
}