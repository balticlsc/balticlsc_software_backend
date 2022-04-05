using System.Collections.Generic;
using Baltic.DataModel.CALExecutable;
using Baltic.DataModel.Execution;
using Baltic.Types.DataAccess;
using Microsoft.AspNetCore.Routing;
using Serilog;

namespace Baltic.TaskRegistry.DataAccess
{
    public class TaskManagementDaoImplMock : ITaskManagement
    {
        private List<CTask> _storedTasks;
        
        public TaskManagementDaoImplMock(TaskRegistryMock trm)
        {
            _storedTasks = trm.StoredTasks;
        }

        /// 
        /// <param name="task"></param>
        public void StoreTask(CTask task)
        {
            _storedTasks.Add(task);
        }

        /// 
        /// <param name="taskUid"></param>
        public CTask GetTask(string taskUid)
        {
            return _storedTasks.Find(t => t.Uid == taskUid);
        }

        /// 
        /// <param name="data"></param>
        /// <param name="accessData"></param>
        /// <param name="dataTokenUid"></param>
        public short UpdateDataSet(CDataSet data, CDataSet accessData, string dataTokenUid)
        {
            
            foreach (var task in _storedTasks)
            {
                CDataToken foundTaskToken = null;
                foreach (var taskToken in task.Tokens)
                    if (dataTokenUid == taskToken.Uid)
                    {
                        taskToken.Data = data;
                        taskToken.AccessData = accessData;
                        Log.Debug($"Set access parameters for task TokenNo={taskToken.TokenNo} to: " +
                                  $"{taskToken.AccessData?.Values??""}");
                        foundTaskToken = taskToken;
                        break;
                    }

                foreach (var batch in task.Batches)
                {
                    foreach (var batchToken in batch.Tokens)
                        if (foundTaskToken?.TokenNo == batchToken.TokenNo || batchToken.Uid == dataTokenUid)
                        {
                            batchToken.Data = data;
                            batchToken.AccessData = accessData;
                            Log.Debug($"Set access parameters for batch TokenNo=" +
                                      $"{batchToken.TokenNo} to: {batchToken.AccessData?.Values??""}");

                            foreach (CJob job in batch.Jobs)
                            {
                                foreach (CDataToken jobToken in job.Tokens)
                                    if (batchToken.TokenNo == jobToken.TokenNo)
                                    {
                                        jobToken.Data = data;
                                        jobToken.AccessData = accessData;
                                        Log.Debug($"Set access parameters for job TokenNo=" +
                                                  $"{jobToken.TokenNo} to: {jobToken.AccessData?.Values}");
                                        return 0;
                                    }
                            }
                        }
                }
            }
            return -1;
        }

        public IEnumerable<ResourceUsage> GetResourceUsage(UsageQuery query)
        {
            return null;
        }

        public IEnumerable<CDataToken> GetTaskDataTokens(string taskUid)
        {
            CTask task = GetTask(taskUid);
            if (null == task)
                return null;
            if (0 != task.Tokens.Count)
                return task.Tokens;
            return task.Batches[0].Tokens;
        }

        public CJobBatch GetJobBatch(string batchUid)
        {
            CJobBatch result;
            foreach (CTask t in _storedTasks)
                if (null != (result = t.Batches.Find(b => b.Uid == batchUid)))
                    return result;
            return null;
        }

        public CJob GetJob(string jobUid)
        {
            throw new System.NotImplementedException();
        }

        public IEnumerable<CTask> FindTasks(TaskQuery query)
        {
            List<CTask> result = _storedTasks;
            if (!string.IsNullOrEmpty(query.AppUid))
                result = result.FindAll(t => t.ReleaseUid == query.AppUid);
            if (query.IsPrivate)
                result = result.FindAll(t => null != t.Execution && t.Execution.Parameters.IsPrivate);
            if (!string.IsNullOrEmpty(query.UserUid))
                result = result.FindAll(t => t.OwnerUid == query.UserUid);
            if (null != query.Statuses && 0 != query.Statuses.Count)
                result = result.FindAll(t => null != t.Execution && query.Statuses.Contains(t.Execution.Status));
            // TODO
            return result;
        }
    }
}