using System.Collections.Generic;
using Baltic.DataModel.Execution;
using Baltic.DataModel.Resources;
using Baltic.Types.DataAccess;

namespace Baltic.NetworkRegistry.DataAccess
{
    public class NetworkBrokerageDaoImpl : INetworkBrokerage
    {

        /// 
        /// <param name="range"></param>
        public List<CCluster> GetMatchingClusters(ResourceReservationRange range)
        {
            var cl = new List<CCluster> {new CCluster()};
            return cl;
        }

        public CCluster GetCluster(string clusterUid)
        {
            throw new System.NotImplementedException();
        }
    }
}