using System.Collections.Generic;
using Baltic.DataModel.Accounts;
using Baltic.DataModel.Types;

namespace Baltic.DataModel.Resources
{
    public class CCluster
    {
        public string Uid { get; set; }
        public string Name { get; set; }
        public string Country { get; set; }
        public string NodeUid { get; set; }
        public ClusterStatus Status { get; set; }

        /// <summary>
        /// Probability that the Cluster stays active (0-100%)
        /// </summary>
        public short AvailabilityLevel { get; set; }

        public ClusterPrivacy Privacy { get; set; }
        public ClusterManifest Manifest { get; set; }
        public List<CMachine> Machines { get; set; }
        public List<ClusterMetric> Performance { get; set; }
        public Organisation Owner { get; set; }
    }
}