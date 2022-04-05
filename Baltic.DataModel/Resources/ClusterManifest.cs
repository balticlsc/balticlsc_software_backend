using System.Collections.Generic;

namespace Baltic.DataModel.Resources
{
    public class ClusterManifest
    {
        public List<string> LocalStorage { get; set; }
        public string LanThroughput { get; set; }
        public string WanThroughput { get; set; }
    }
}