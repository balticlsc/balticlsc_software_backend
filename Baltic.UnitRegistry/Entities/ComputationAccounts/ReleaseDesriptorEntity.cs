using System;

namespace UnitManager.Entity.ComputationAccounts
{
    public class ReleaseDescriptorEntity
    {
        public int Id { get; set; }
        public DateTime Date { get; set; }
        public string Description { get; set; }
        public bool OpenSource { get; set; }
        public long UsageCounter { get; set; }
        public int ComputationUnitReleaseId { get; set; } // Descriptor : ReleaseDescriptor , wynika z kompozycji
    }
}