using System;

namespace Baltic.TaskRegistry.Entities
{
    public class ResourceUsageEntity
    {
        public class CTaskEntity
        {
            public int Id { get; set; }
            public DateTime Stamp { get; set; }
            public int JobExecutionId { get; set; }
            
            public DateTime TimeStamp { get; set; }
            public int Kind { get; set; } //(enum: ResourceKind)
            public float Value { get; set; }
        }
    }
}
