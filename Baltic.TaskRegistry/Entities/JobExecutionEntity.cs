using System;

namespace Baltic.TaskRegistry.Entities
{
    public class JobExecutionEntity
    {
        public int Id { get; set; }
        public int CJobId { get; set; }
        public int BatchExecutionId { get; set; }
        public DateTime Stamp { get; set; }

        public string MsgUid { get; set; }
        public int Progress { get; set; }
        public float EstimatedTime { get; set; }
        
        //from: ExecutionRecord
        public int Status { get; set; } //(enum: ComputationStatus)
        public DateTime Start { get; set; }
        public DateTime Finish { get; set; }
    }
}
