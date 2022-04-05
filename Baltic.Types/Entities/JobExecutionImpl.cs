using Baltic.DataModel.Execution;

namespace Baltic.Types.Entities
{
    public class JobExecutionImpl : JobExecution
    {
        public string JobUid { get; set; }
        public string JobInstanceUid { get; set; }
        public string BatchExecutionUid { get; set; }
    }
}