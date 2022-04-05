using System;
using System.Collections.Generic;
using Baltic.DataModel.CALExecutable;
using Baltic.DataModel.Resources;

namespace Baltic.DataModel.Execution
{
    public abstract class BatchExecution : InstantiableExecution
    {
        /// <summary>
        /// Used as the Uid of the BatchExecution
        /// </summary>
        public string BatchMsgUid { get; set; }
        public float DataTransfer { get; set; }
        public DateTime StorageFinish { get; set; }
        public float EstimatedCredits { get; set; }
        public float ConsumedCredits { get; set; }
        public float ConsumedStorageCredits { get; set; }
        public ResourceReservation ActualReservation { get; set; }
        public CCluster Cluster { get; set; }
        public List<JobExecution> JobExecutions { get; set; }
        public List<JobInstance> JobInstances { get; set; }
        public CJobBatch Batch { get; set; }
    }
}