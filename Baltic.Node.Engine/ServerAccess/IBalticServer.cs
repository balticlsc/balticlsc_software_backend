using System.Collections.Generic;
using Baltic.DataModel.CALExecutable;
using Baltic.DataModel.CALMessages;
using Baltic.DataModel.Execution;

namespace Baltic.Node.Engine.ServerAccess
{
	public interface IBalticServer
	{
		/// 
		/// <param name="msgUids"></param>
		/// <param name="status"></param>
		/// <param name="isFinal"></param>
		/// <param name="isFailed"></param>
		/// <param name="note"></param>
		short AckMessages(Dictionary<string, QueueId> msgUids, FullJobStatus status, bool isFinal, bool isFailed, string note);
		
		/// 
		/// <param name="tm"></param>
		short PutTokenMessage(TokenMessage tm);

		/// 
		/// <param name="batchMsgUid"></param>
		/// <param name="jobQueueIds"></param>
		void ConfirmBatchStart(string batchMsgUid, List<QueueId> jobQueueIds);

		/// 
		/// <param name="instanceUid"></param>
		/// <param name="requiredPinQueues"></param>
		/// <param name="isNewInstance"></param>
		void ConfirmJobStart(string instanceUid, List<QueueId> requiredPinQueues, bool isNewInstance);

	}
}