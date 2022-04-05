using Baltic.DataModel.CALExecutable;
using Baltic.DataModel.CALMessages;

namespace Baltic.Types.QueueAccess
{
	public interface IQueueConsumer {

		/// 
		/// <param name="msg"></param>
		short MessageReceived(Message msg);
	}
}