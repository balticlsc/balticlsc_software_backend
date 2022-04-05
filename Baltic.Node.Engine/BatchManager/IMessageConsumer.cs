using Baltic.DataModel.CALExecutable;
using Baltic.DataModel.CALMessages;

namespace Baltic.Node.Engine.BatchManager {
	public interface IMessageConsumer  {
		
		/// 
		/// <param name="bim"></param>
		short BatchInstanceMessageReceived(BatchInstanceMessage bim);
		
		/// 
		/// <param name="bem"></param>
		short BatchExecutionMessageReceived(BatchExecutionMessage bem);

		/// 
		/// <param name="tm"></param>
		short TokenMessageReceived(TokenMessage tm);

		/// 
		/// <param name="jim"></param>
		short JobInstanceMessageReceived(JobInstanceMessage jim);
		
		/// 
		/// <param name="jem"></param>
		short JobExecutionMessageReceived(JobExecutionMessage jem);
	}
}