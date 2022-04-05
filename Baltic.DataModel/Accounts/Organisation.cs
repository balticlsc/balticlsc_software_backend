using System.Collections.Generic;
using Baltic.DataModel.Resources;

namespace Baltic.DataModel.Accounts
{
    public class Organisation
    {
        public string Uid { get; set; }
        public string Name { get; set; }
        public bool IsSupplier { get; set; }
        public string Details { get; set; }
        public IList<UserAccount> Accounts { get; set; }
        public IList<BillingEntry> BillingInformation { get; set; }
        public IList<CCluster> OwnedClusters { get; set; }
    }
}