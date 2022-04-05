using System.Collections.Generic;
using Baltic.DataModel.Execution;

namespace Baltic.Engine.JobBroker
{
	public interface IBalticNode  {

		/// 
		/// <param name="batchMsgUid"></param>
		List<FullJobStatus> GetBatchJobStatuses(string batchMsgUid);

		/// 
		/// <param name="jobInstanceUid"></param>
		short FinishJobInstance(string jobInstanceUid);
		
		/// 
		/// <param name="jobInstanceUid"></param>
		short FinishJobExecution(string jobInstanceUid);

		/// 
		/// <param name="batchMsgUid"></param>
		short FinishJobBatch(string batchMsgUid);

	}
}