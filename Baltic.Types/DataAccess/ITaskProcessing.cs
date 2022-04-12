using System.Collections.Generic;
using Baltic.DataModel.CALExecutable;
using Baltic.DataModel.CALMessages;
using Baltic.DataModel.Types;

namespace Baltic.Types.DataAccess
{
    public interface ITaskProcessing 
    {
        /// 
        /// <param name="tm"></param>
        CJobBatch GetJobBatchForToken(TokenMessage tm);

        /// 
        /// <param name="tm"></param>
        CJobBatch GetJobBatchForRequiredToken(TokenMessage tm);

        /// 
        /// <param name="tm"></param>
        CJobBatch GetJobBatchForProvidedToken(TokenMessage tm);

        /// 
        /// <param name="tm"></param>
        CJob GetJobForRequiredToken(TokenMessage tm);

        /// 
        /// <param name="taskUid"></param>
        /// <param name="tokenNo"></param>
        CJob GetJobForRequiredToken(string taskUid, long tokenNo);

        ///
        /// <param name="job"></param>
        /// <param name="taskUid"></param>
        CJobBatch GetJobBatchForJob(CJob job, string taskUid);
        
        /// 
        /// <param name="taskUid"></param>
        List<CDataToken> GetTaskDataTokens(string taskUid);

        /// 
        /// <param name="tm"></param>
        bool IsTaskProvidedToken(TokenMessage tm);

        /// 
        /// <param name="taskUid"></param>
        /// <param name="tokenNo"></param>
        bool IsTaskProvidedToken(string taskUid, long tokenNo);

        /// 
        /// <param name="tm"></param>
        bool IsTaskRequiredToken(TokenMessage tm);

        /// 
        /// <param name="queueName"></param>
        /// <param name="jobInstanceUid"></param>
        short AddJobForEmptiedQueue(QueueId queueName, string jobInstanceUid);
        
        /// 
        /// <param name="taskUid"></param>
        short ClearEmptiedQueuesToJobs(string taskUid);

        /// 
        /// <param name="queueName"></param>
        List<string> GetJobsForEmptiedQueue(QueueId queueName);

        /// 
        /// <param name="taskUid"></param>
        bool IsTaskWorking(string taskUid);

        /// 
        /// <param name="taskUid"></param>
        FailureHandlingPolicy GetTaskFHPolicy(string taskUid);

    }
}