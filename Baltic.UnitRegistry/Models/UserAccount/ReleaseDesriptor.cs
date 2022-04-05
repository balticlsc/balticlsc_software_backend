using System;

namespace UnitManager.DTO.ComputationAccounts
{
    public class ReleaseDescriptor 
    {

        public DateTime Date { get; set; }
        public string Description { get; set; }
        public bool OpenSource { get; set; }
        public long UsageCounter { get; set; }
    }
}