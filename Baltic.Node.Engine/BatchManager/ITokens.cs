using System.Collections.Generic;
using Baltic.DataModel.CALExecutable;
using Baltic.DataModel.CALMessages;

namespace Baltic.Node.Engine.BatchManager
{
	public interface ITokens    {

		///
		/// <param name="tm"></param>
		/// <param name="requiredMsgUid"></param>
		/// <param name="finalMsg"></param>
		short PutTokenMessage(TokenMessage tm, string requiredMsgUid, bool finalMsg);

		/// 
		/// <param name="msgUids"></param>
		/// <param name="senderUid"></param>
		/// <param name="isFinal"></param>
		/// <param name="isFailed"></param>
		/// <param name="note"></param>
		short AckTokenMessages(List<string> msgUids, string senderUid, bool isFinal, bool isFailed, string note);

	}
}