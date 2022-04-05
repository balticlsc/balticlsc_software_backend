using Baltic.DataModel.Resources;
using Baltic.Types.QueueAccess;

namespace Baltic.Engine.JobBroker
{
	public interface IClusterNodeAccessFactory
	{
		
		/// 
		/// <param name="cluster"></param>
		IBalticNode CreateClusterNodeAccess(CCluster cluster);

		/// 
		/// <param name="cluster"></param>
		IQueueConsumer CreateQueueConsumerAccess(CCluster cluster);
	}
}