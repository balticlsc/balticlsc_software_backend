using Baltic.DataModel.Types;

namespace Baltic.DataModel.Accounts
{
    public class BillingEntry
    {
        public string Uid { get; set; }
        public BillingType Type { get; set; }
        public BillingDirection Direction { get; set; }
        public string Details { get; set; }
    }
}