using System.Collections.Generic;
using Baltic.DataModel.CALExecutable;
using Baltic.DataModel.Resources;
using Baltic.DataModel.Types;

namespace Baltic.Types.DataAccess {
	public interface INetworkManagement  {

		/// <summary>
		/// returns clusterUid
		/// </summary>
		/// <param name="cluster"></param>
		string CreateCluster(CCluster cluster);

		/// 
		/// <param name="cluster"></param>
		short UpdateCluster(CCluster cluster);

		/// 
		/// <param name="clusterUid"></param>
		/// <param name="status"></param>
		short UpdateClusterStatus(string clusterUid, ClusterStatus status);

		/// 
		/// <param name="query"></param>
		IEnumerable<CCluster> FindClusters(ClusterQuery query);

		/// 
		/// <param name="userUid"></param>
		IEnumerable<CCluster> GetUserClusters(string userUid);

		/// 
		/// <param name="clusterUid"></param>
		CCluster GetCluster(string clusterUid);

		/// 
		/// <param name="machineUid"></param>
		CMachine GetMachine(string machineUid);

		/// 
		/// <param name="query"></param>
		IEnumerable<CJobBatch> GetBatchesForResource(ClusterBatchQuery query);
	}
}