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
            Clusters.Add(
                new CCluster()
                {
                    Name = "LocalHost Alternative",
                    Uid = Guid.NewGuid().ToString(),
                    NodeUid = "00155D9519AB-3154C0B8-A2691D20"
                }
            );
            Clusters.Add(
                new CCluster()
                {
                    Name = "Lulea, Sweden (RISE)",
                    Uid = Guid.NewGuid().ToString(),
                    NodeUid = "TEST01234567-NODE1234-RISE1234"
                }
            );
            Clusters.Add(
                new CCluster()
                {
                    Name = "Turku, Finland (MTC)",
                    Uid = Guid.NewGuid().ToString(),
                    NodeUid = "TEST01234567-NODE1234-TURKU123"
                }
            );
            Clusters.Add(
                new CCluster()
                {
                    Name = "Warsaw, Poland (WUT)",
                    Uid = Guid.NewGuid().ToString(),
                    NodeUid = "TEST01234567-NODE1234-PWEE1234"
                }
            );
        }
    }
}