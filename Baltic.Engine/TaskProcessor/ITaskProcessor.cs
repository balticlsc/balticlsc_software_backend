using System.Collections.Generic;
using Baltic.DataModel.CALExecutable;
using Baltic.DataModel.CALMessages;

namespace Baltic.Engine.TaskProcessor
{
	public interface ITaskProcessor
	{
		/// 
		/// <param name="tm"></param>
		short PutTokenMessage(TokenMessage tm);

		/// 
		/// <param name="msgUids"></param>
		/// <param name="jobInstanceUid"></param>
		/// <param name="isFinal"></param>
		short AckMessages(Dictionary<string, QueueId> msgUids, string jobInstanceUid, bool isFinal, bool isFailed);

		short AbortTask(string taskUid);
	}
}
