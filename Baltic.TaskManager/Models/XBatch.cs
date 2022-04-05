using System;
using System.Collections.Generic;
using Baltic.DataModel.Types;

namespace Baltic.TaskManager.Models
{
    public class XBatch
    {
        // from: CJobBatch
        public string Uid { get; set; }
        public int DepthLevel { get; set; }

        // from: ExecutionRecord
        public int SerialNo { get; set; }
        public IEnumerable<XJob> Jobs { get; set; }

        // from: ExecutionRecord
        public ComputationStatus Status { get; set; }
        public DateTime Start { get; set; }
        public DateTime Finish { get; set; }

        // from: BatchExecution
        public string BatchMsgUid { get; set; }
        public float DataTransfer { get; set; }
        public DateTime StorageFinish { get; set; }
        public float EstimatedCredits { get; set; }
        public float ConsumedCredits { get; set; }
        public float ConsumedStorageCredits { get; set; }
        public long TokensReceived { get; set; }
        public long TokensProcessed { get; set; }
        public XClusterBasic Cluster { get; set; }
    }
}