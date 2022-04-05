using System.Collections.Generic;
using Baltic.DataModel.Execution;
using Baltic.DataModel.Resources;

namespace Baltic.Types.DataAccess
{
    public interface INetworkBrokerage
    {
        /// 
        /// <param name="range"></param>
        List<CCluster> GetMatchingClusters(ResourceReservationRange range);

        /// 
        /// <param name="clusterUid"></param>
        CCluster GetCluster(string clusterUid);
    }
}