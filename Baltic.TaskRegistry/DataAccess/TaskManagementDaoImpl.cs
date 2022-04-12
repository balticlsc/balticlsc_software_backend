using System.Collections.Generic;
using Baltic.DataModel.CALExecutable;
using Baltic.DataModel.Execution;
using Baltic.Types.DataAccess;

namespace Baltic.TaskRegistry.DataAccess
{
    public class TaskManagementDaoImpl : ITaskManagement
    {
        private List<CTask> _storedTasks;
        
        public void Init(TaskRegistryMock trm)
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
                foreach (var dt in task.Tokens)
                    if (dataTokenUid == dt.PinUid)
                    {
                        dt.Data = data;
                        dt.AccessData = accessData;
                        return 0;
                    }

                foreach (var batch in task.Batches)
                foreach (var dt in batch.Tokens)
                    if (dataTokenUid == dt.PinUid)
                    {
                        dt.Data = data;
                        dt.AccessData = accessData;
                        return 0;
                    }
//				foreach (CJob job in batch.jobs) {
//					foreach (CDataToken dt in job.tokens)
//						if (data_token_uid == dt.pin_uid) {
//							dt.data_set = parameters;
//                          dt.AccessData = accessParameters;
//							return 0;
//						}
//				}
            }
            return -1;
        }

        public IEnumerable<ResourceUsage> GetResourceUsage(UsageQuery query)
        {
            return null;
        }

        public IEnumerable<CDataToken> GetTaskDataTokens(string taskUid)
        {
            throw new System.NotImplementedException();
        }

        public void AbortTask(string taskUid)
        {
            throw new System.NotImplementedException();
        }

        public CJobBatch GetJobBatch(string batchUid)
        {
            throw new System.NotImplementedException();
        }

        public CJob GetJob(string jobUid)
        {
            throw new System.NotImplementedException();
        }

        public IEnumerable<CTask> FindTasks(TaskQuery query)
        {
            throw new System.NotImplementedException();
        }

        public short ArchiveTask(string taskUid)
        {
            throw new System.NotImplementedException();
        }
    }
}