using System.Collections.Generic;
using Baltic.DataModel.CALExecutable;
using Baltic.DataModel.Resources;
using Baltic.DataModel.Types;
using Baltic.Types.DataAccess;

namespace Baltic.NetworkRegistry.DataAccess {
	public class NetworkManagementDaoImpl : INetworkManagement {

		/// <summary>
		/// returns clusterUid
		/// </summary>
		/// <param name="cluster"></param>
		public string CreateCluster(CCluster cluster){

			return "";
		}

		/// 
		/// <param name="cluster"></param>
		public short UpdateCluster(CCluster cluster)
		{
			return 0;
		}

		/// 
		/// <param name="clusterUid"></param>
		/// <param name="status"></param>
		public short UpdateClusterStatus(string clusterUid, ClusterStatus status)
		{
			return 0;
		}

		/// 
		/// <param name="query"></param>
		public IEnumerable<CCluster> FindClusters(ClusterQuery query){

			return null;
		}

		/// 
		/// <param name="userUid"></param>
		public IEnumerable<CCluster> GetUserClusters(string userUid){

			return null;
		}

		/// 
		/// <param name="clusterUid"></param>
		public CCluster GetCluster(string clusterUid){

			return null;
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