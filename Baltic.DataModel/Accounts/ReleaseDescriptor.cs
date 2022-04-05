using System;

namespace Baltic.DataModel.Accounts
{
    public class ReleaseDescriptor
    {
        public DateTime Date { get; set; }
        public string Description { get; set; }
        public bool IsOpenSource { get; set; }
        public long UsageCounter { get; set; }
    }
}