using Baltic.DataModel.CALExecutable;
using Baltic.DataModel.CALMessages;
using Baltic.DataModel.Execution;

namespace Baltic.Node.Engine.DataAccess
{
	public interface IJob  {

		/// 
		/// <param name="tm"></param>
		short ProcessTokenMessage(TokenMessage tm);

		JobStatus GetStatus();
	}
}