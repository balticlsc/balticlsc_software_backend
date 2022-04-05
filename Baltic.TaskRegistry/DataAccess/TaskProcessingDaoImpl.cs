using System.Collections.Generic;
using Baltic.DataModel.CALExecutable;
using Baltic.DataModel.CALMessages;
using Baltic.DataModel.Types;
using Baltic.Types.DataAccess;

namespace Baltic.TaskRegistry.DataAccess
{
    public class TaskProcessingDaoImpl : ITaskProcessing
    {
        private List<CTask> _storedTasks;

        public void Init(TaskRegistryMock trm)
        {
            _storedTasks = trm.StoredTasks;
        }

        /// 
        /// <param name="tm"></param>
        public CJobBatch GetJobBatchForToken(TokenMessage tm)
        {
            foreach (var task in _storedTasks)
                if (tm.TaskUid == task.Uid)
                    foreach (var batch in task.Batches)
                    foreach (var dt in batch.Tokens)
                        if (tm.TokenNo == dt.TokenNo || -tm.TokenNo == dt.TokenNo)
                            return batch;
            return null;
        }

        /// 
        /// <param name="tm"></param>
        public CJobBatch GetJobBatchForRequiredToken(TokenMessage tm)
        {
            foreach (var task in _storedTasks)
                if (tm.TaskUid == task.Uid)
                    foreach (var batch in task.Batches)
                    foreach (var dt in batch.Tokens)
                        if ((tm.TokenNo == dt.TokenNo || -tm.TokenNo == dt.TokenNo)
                            && dt.Binding < DataBinding.Provided)
                            return batch;
            return null;
        }

        /// 
        /// <param name="tm"></param>
        public CJobBatch GetJobBatchForProvidedToken(TokenMessage tm)
        {
            foreach (var task in _storedTasks)
                if (tm.TaskUid == task.Uid)
                    foreach (var batch in task.Batches)
                    foreach (var dt in batch.Tokens)
                        if (tm.TokenNo == dt.TokenNo && dt.Binding >= DataBinding.Provided)
                            return batch;
            return null;
        }

        /// 
        /// <param name="tm"></param>
        public CJob GetJobForRequiredToken(TokenMessage tm)
        {
            foreach (var task in _storedTasks)
                if (tm.TaskUid == task.Uid)
                    foreach (var batch in task.Batches)
                    foreach (var j in batch.Jobs)
                    foreach (var dt in j.Tokens)
                        if (tm.TokenNo == dt.TokenNo && dt.Binding < DataBinding.Provided)
                            return j;
            return null;
        }

        public CJob GetJobForRequiredToken(string taskUid, long tokenNo)
        {
            throw new System.NotImplementedException();
        }

        /// 
        /// <param name="job"></param>
        /// <param name="taskUid"></param>
        public CJobBatch GetJobBatchForJob(CJob job, string taskUid)
        {
            foreach (var task in _storedTasks)
                if (null == taskUid || task.Uid == taskUid)
                    foreach (var batch in task.Batches)
                        if (job.Batch.Uid == batch.Uid)
                            return batch;
            return null;
        }

        public List<CDataToken> GetTaskDataTokens(string taskUid)
        {
            throw new System.NotImplementedException();
        }

        /// 
        /// <param name="tm"></param>
        public bool IsTaskProvidedToken(TokenMessage tm)
        {
            foreach (var task in _storedTasks)
                if (tm.TaskUid == task.Uid)
                {
                    if (1 == task.Batches.Count)
                    {
                        return DataBinding.Provided <= task.Batches[0].Tokens
                            .Find(dt => dt.TokenNo == tm.TokenNo || -dt.TokenNo == tm.TokenNo).Binding;
                    }
                    else
                    {
                        var mainDT = task.Tokens.Find(dt => dt.TokenNo == tm.TokenNo || -dt.TokenNo == tm.TokenNo);
                        return null != mainDT && DataBinding.Provided <= mainDT.Binding;
                    }
                }

            return false;
        }

        public bool IsTaskProvidedToken(string taskUid, long tokenNo)
        {
            throw new System.NotImplementedException();
        }

        public bool IsTaskRequiredToken(TokenMessage tm)
        {
            throw new System.NotImplementedException();
        }
        
        public short AddJobForEmptiedQueue(QueueId queueName, string jobInstanceUid)
        {
            throw new System.NotImplementedException();
        }

        public short ClearEmptiedQueuesToJobs(string taskUid)
        {
            throw new System.NotImplementedException();
        }

        public List<string> GetJobsForEmptiedQueue(QueueId queueName)
        {
            throw new System.NotImplementedException();
        }

        public bool IsTaskWorking(string taskUid)
        {
            throw new System.NotImplementedException();
        }
        
    }
}