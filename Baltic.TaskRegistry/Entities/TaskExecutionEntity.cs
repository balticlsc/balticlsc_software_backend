using System;

namespace Baltic.TaskRegistry.Entities
{
    public class TaskExecutionEntity
    {
        public int Id { get; set; }
        public int CTaskId { get; set; }
        public int? MinResourceReservationId { get; set; }
        public int? MaxResourceReservationId { get; set; }
        public DateTime Stamp { get; set; }

        public float ConsumedCredits { get; set; }
        public bool IsArchived { get; set; }
        
        //from: ExecutionRecord
        public int Status { get; set; } //(enum: ComputationStatus)
        public DateTime Start { get; set; }
        public DateTime Finish { get; set; }
        
        //from: TaskParameters
        public string TaskName { get; set; }
        public int Priority { get; set; }
        public int ClusterAllocation { get; set; } //(enum: UnitStrength)
        public float ReservedCredits { get; set; }
        public float AuxStorageCredits { get; set; }
        public bool IsPrivate { get; set; }
    }
}
