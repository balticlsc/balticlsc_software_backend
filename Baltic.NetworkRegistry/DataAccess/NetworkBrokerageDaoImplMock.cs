using System;
using System.Collections.Generic;
using Baltic.CommonServices;
using Baltic.DataModel.Execution;
using Baltic.DataModel.Resources;
using Baltic.Types.DataAccess;

namespace Baltic.NetworkRegistry.DataAccess
{
    public class NetworkBrokerageDaoImplMock : INetworkBrokerage
    {
        private List<CCluster> _clusters;

        public NetworkBrokerageDaoImplMock(NetworkRegistryMock mock, NodeManager nm)
        {
            _clusters = mock.Clusters;
            foreach (string clusterUid in nm.Keys)
                // check if the NodeManager contains a cluster that is not yet in the network registry
                if (!_clusters.Exists(c => c.NodeUid == clusterUid))
                {
                    // add a new cluster to the network registry
                    _clusters.Add(new CCluster()
                    {
                        NodeUid = clusterUid,
                        Name = "cluster_" + (_clusters.Count+1),
                        Uid = Guid.NewGuid().ToString()
                    });
                }
        }

        /// 
        /// <param name="range"></param>
        public List<CCluster> GetMatchingClusters(ResourceReservationRange range)
        {
            return _clusters;
        }

        public CCluster GetCluster(string clusterUid)
        {
            return _clusters.Find(cl => cl.Uid == clusterUid);
        }
    }
}