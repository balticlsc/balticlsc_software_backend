using Baltic.DataModel.CALExecutable;

namespace Baltic.DataModel.Execution
{
    public abstract class JobExecution : InstantiableExecution
    {
        public string JobMsgUid { get; set; }
        public long Progress { get; set; }
        public float EstimatedTime { get; set; }
        public long TokensReceived { get; set; }
        public long TokensProcessed { get; set; }
        public string Note { get; set; }
        public CJob Job { get; set; }
        public BatchExecution BatchExecution { get; set; }
        public JobInstance Instance { get; set; }
    }
}