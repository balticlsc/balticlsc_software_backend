using System;
using Baltic.DataModel.Types;

namespace Baltic.TaskManager.Models
{
    public class XJob
    {
        // from: CJob
        public string Uid { get; set; }
        public string UnitUid { get; set; }
        public int Multiplicity { get; set; }
        public string CallName { get; set; }
        public string ModuleName { get; set; }

        // from: ExecutionRecord
        public ComputationStatus Status { get; set; }
        public DateTime Start { get; set; }
        public DateTime Finish { get; set; }

        // from: JobExecution
        public string JobMsgUid { get; set; }
        public string JobInstanceUid { get; set; }
        public long Progress { get; set; }
        public float EstimatedTime { get; set; }
        public long TokensReceived { get; set; }
        public long TokensProcessed { get; set; }
        public string Note { get; set; }
    }
}