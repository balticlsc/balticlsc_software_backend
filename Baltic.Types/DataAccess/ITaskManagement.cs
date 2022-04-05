using System.Collections.Generic;
using Baltic.DataModel.CALExecutable;
using Baltic.DataModel.Execution;

namespace Baltic.Types.DataAccess
{
    public interface ITaskManagement
    {
        /// 
        /// <param name="task"></param>
        void StoreTask(CTask task);

        /// 
        /// <param name="taskUid"></param>
        CTask GetTask(string taskUid);

        /// 
        /// <param name="data"></param>
        /// <param name="accessData"></param>
        /// <param name="dataTokenUid"></param>
        short UpdateDataSet(CDataSet data, CDataSet accessData, string dataTokenUid);

        /// 
        /// <param name="query"></param>
        IEnumerable<ResourceUsage> GetResourceUsage(UsageQuery query);

        /// 
        /// <param name="taskUid"></param>
        IEnumerable<CDataToken> GetTaskDataTokens(string taskUid);
        
        /// 
        /// <param name="batchUid"></param>
        CJobBatch GetJobBatch(string batchUid);

        /// 
        /// <param name="jobUid"></param>
        CJob GetJob(string jobUid);

        /// 
        /// <param name="query"></param>
        IEnumerable<CTask> FindTasks(TaskQuery query);
    }
}