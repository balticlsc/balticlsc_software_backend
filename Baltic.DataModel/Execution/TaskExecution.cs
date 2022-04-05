using Baltic.DataModel.CALExecutable;

namespace Baltic.DataModel.Execution
{
    public abstract class TaskExecution : ExecutionRecord
    {
        public float ConsumedCredits { get; set; }
        public bool IsArchived { get; set; }
        public TaskParameters Parameters { get; set; }
        public CTask Task { get; set; }
    }
}