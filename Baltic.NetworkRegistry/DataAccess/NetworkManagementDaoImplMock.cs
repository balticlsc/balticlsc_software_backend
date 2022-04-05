using System;
using System.Collections.Generic;
using System.Linq;
using Baltic.Core.Utils;
using Baltic.DataModel.CALExecutable;
using Baltic.DataModel.Resources;
using Baltic.DataModel.Types;
using Baltic.Types.DataAccess;

namespace Baltic.NetworkRegistry.DataAccess {
	public class NetworkManagementDaoImplMock : INetworkManagement {
		
		private List<CCluster> _clusters;

		public NetworkManagementDaoImplMock(NetworkRegistryMock mock)
		{
			_clusters = mock.Clusters;
		}

		/// <summary>
		/// returns clusterUid
		/// </summary>
		/// <param name="cluster"></param>
		public string CreateCluster(CCluster cluster)
		{
			if (_clusters.Exists(c => c.NodeUid == cluster.NodeUid))
				return null;
			cluster.Uid = Guid.NewGuid().ToString();
			_clusters.Add(cluster);
			return cluster.Uid;
		}

		/// 
		/// <param name="cluster"></param>
		public short UpdateCluster(CCluster cluster)
		{
			CCluster foundCluster = _clusters.Find(c => c.Uid == cluster.Uid);
			if (null == foundCluster)
				return -1;
			DBMapper.Map<CCluster>(cluster, foundCluster);
			// TODO - update also the embedded lists
			return 0;
		}

		/// 
		/// <param name="clusterUid"></param>
		/// <param name="status"></param>
		public short UpdateClusterStatus(string clusterUid, ClusterStatus status)
		{
			CCluster foundCluster = _clusters.Find(c => c.Uid == clusterUid);
			if (null == foundCluster)
				return -1;
			foundCluster.Status = status;
			return 0;
		}

		/// 
		/// <param name="query"></param>
		public IEnumerable<CCluster> FindClusters(ClusterQuery query)
		{
			return _clusters;
		}

		/// 
		/// <param name="userUid"></param>
		public IEnumerable<CCluster> GetUserClusters(string userUid)
		{
			return _clusters.FindAll(c => null != c.Owner && c.Owner.Accounts.ToList().Exists(u => u.Uid == userUid));
		}

		/// 
		/// <param name="clusterUid"></param>
		public CCluster GetCluster(string clusterUid)
		{
			return _clusters.Find(c => c.Uid == clusterUid);
		}

		/// 
		/// <param name="machineUid"></param>
		public CMachine GetMachine(string machineUid){

			return null;
		}

		/// 
		/// <param name="query"></param>
		public IEnumerable<CJobBatch> GetBatchesForResource(ClusterBatchQuery query){

			return null;
		}

	}
}