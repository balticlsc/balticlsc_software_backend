using System;

namespace Baltic.TaskRegistry.Entities
{
    public class BatchExecutionEntity
    {
        public int Id { get; set; }
        public int CJobBatchId { get; set; }
        public int? ActualReservationId { get; set; }
        public int CClusterId { get; set; }
        public DateTime Stamp { get; set; }

        public string MsgUid { get; set; }
        public float DataTransfer { get; set; }
        public DateTime StorageFinish { get; set; }
        public float EstimatedCredits { get; set; }
        public float ConsumedCredits { get; set; }
        public float ConsumedStorageCredits { get; set; }

        //from: ExecutionRecord
        public int Status { get; set; } //(enum: ComputationStatus)
        public DateTime Start { get; set; }
        public DateTime Finish { get; set; }
    }
}
