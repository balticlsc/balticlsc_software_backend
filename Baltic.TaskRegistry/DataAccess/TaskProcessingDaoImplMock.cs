using System;
using System.Collections.Generic;
using Baltic.DataModel.CALExecutable;
using Baltic.DataModel.CALMessages;
using Baltic.DataModel.Types;
using Baltic.Types.DataAccess;

namespace Baltic.TaskRegistry.DataAccess
{
    public class TaskProcessingDaoImplMock : ITaskProcessing
    {
        private List<CTask> _storedTasks;
        private IDictionary<QueueId, List<string>> _emptiedQueuesToJobs;

        public TaskProcessingDaoImplMock(TaskRegistryMock trm)
        {
            _storedTasks = trm.StoredTasks;
            _emptiedQueuesToJobs = trm.EmptiedQueuesToJobs;
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
            return GetJobForRequiredToken(tm.TaskUid, tm.TokenNo);
        }

        /// 
        /// <param name="taskUid"></param>
        /// <param name="tokenNo"></param>
        public CJob GetJobForRequiredToken(string taskUid, long tokenNo)
        {
            foreach (var task in _storedTasks)
                if (taskUid == task.Uid)
                    foreach (var batch in task.Batches)
                    foreach (var j in batch.Jobs)
                    foreach (var dt in j.Tokens)
                        if (tokenNo == dt.TokenNo && dt.Binding < DataBinding.Provided)
                            return j;
            return null;
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
            foreach (var task in _storedTasks)
                if (taskUid == task.Uid)
                {
                    CDataToken mainDT;
                    if (1 == task.Batches.Count)
                        return task.Batches[0].Tokens;
                    return task.Tokens;
                }

            return new List<CDataToken>();
        }

        /// 
        /// <param name="tm"></param>
        public bool IsTaskProvidedToken(TokenMessage tm)
        {
            return IsTaskProvidedToken(tm.TaskUid, tm.TokenNo);
        }

        public bool IsTaskProvidedToken(string taskUid, long tokenNo)
        {
            foreach (var task in _storedTasks)
                if (taskUid == task.Uid)
                {
                    CDataToken mainDT;
                    if (1 == task.Batches.Count)
                        mainDT = task.Batches[0].Tokens
                            .Find(dt => dt.TokenNo == tokenNo || -dt.TokenNo == tokenNo);
                    else
                        mainDT = task.Tokens.Find(dt => dt.TokenNo == tokenNo || -dt.TokenNo == tokenNo);
                    return null != mainDT && DataBinding.Provided <= mainDT.Binding;
                }
            return false;
        }

        /// 
        /// <param name="tm"></param>
        public bool IsTaskRequiredToken(TokenMessage tm)
        {
            foreach (var task in _storedTasks)
                if (tm.TaskUid == task.Uid)
                {
                    CDataToken mainDT;
                    if (1 == task.Batches.Count)
                        mainDT = task.Batches[0].Tokens
                            .Find(dt => dt.TokenNo == tm.TokenNo || -dt.TokenNo == tm.TokenNo);
                    else
                        mainDT = task.Tokens.Find(dt => dt.TokenNo == tm.TokenNo || -dt.TokenNo == tm.TokenNo);
                    return null != mainDT && DataBinding.Provided > mainDT.Binding;
                }
            return false;
        }

        public short AddJobForEmptiedQueue(QueueId queueName, string jobInstanceUid)
        {
            _emptiedQueuesToJobs.TryAdd(queueName, new List<string>());
            if (_emptiedQueuesToJobs[queueName].Contains(jobInstanceUid))
                return -1;
            _emptiedQueuesToJobs[queueName].Add(jobInstanceUid);
            return 0;
        }

        public short ClearEmptiedQueuesToJobs(string taskUid)
        {
            // TODO implement
            throw new System.NotImplementedException();
        }

        public List<string> GetJobsForEmptiedQueue(QueueId queueName)
        {
            return _emptiedQueuesToJobs.ContainsKey(queueName) ? _emptiedQueuesToJobs[queueName] : new List<string>();
        }

        public bool IsTaskWorking(string taskUid)
        {
            CTask task = _storedTasks.Find(t => t.Uid == taskUid);
            return null != task && task.Execution.Finish == DateTime.MinValue;
        }
    }
}