using System;
using System.Collections.Generic;
using Baltic.DataModel.Resources;

namespace Baltic.NetworkRegistry.DataAccess
{
    public class NetworkRegistryMock
    {
        public List<CCluster> Clusters = new List<CCluster>();

        public NetworkRegistryMock()
        {
            Clusters.Add(
                new CCluster()
                {
                    Name = "LocalHost",
                    Uid = Guid.NewGuid().ToString(),
                    NodeUid = "00155D9519AB-3154C0B8-A2691D20"
                }
            );
            /* Clusters.Add(
                new CCluster()
                {
                    Name = "LocalHost2",
                    Uid = Guid.NewGuid().ToString(),
                    NodeUid = "00155D9519AB-3154C0B8-A2691D20"
                }
            ); */
        }
    }
}