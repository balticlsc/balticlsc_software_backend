using Baltic.CommonServices;
using Baltic.DataModel.Resources;
using Baltic.Engine.JobBroker;
using Baltic.Types.QueueAccess;

namespace Baltic.TaskManager.Proxies
{
	public class ClusterNodeAccessFactory : IClusterNodeAccessFactory{

		private NodeManager _nodeManager;
		
		public ClusterNodeAccessFactory(NodeManager nodeManager)
		{
			_nodeManager = nodeManager;
		}
		
		/// 
		/// <param name="cluster"></param>
		public IBalticNode CreateClusterNodeAccess(CCluster cluster){
			return new BalticNodeProxy(_nodeManager,cluster.NodeUid);
		}
		
		/// 
		/// <param name="cluster"></param>
		public IQueueConsumer CreateQueueConsumerAccess(CCluster cluster){
			return new BalticNodeProxy(_nodeManager,cluster.NodeUid);
		}

	}
}