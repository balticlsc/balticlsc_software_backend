using Baltic.DataModel.Execution;

namespace Baltic.Types.Entities
{
    public class JobExecutionImpl : JobExecution
    {
        public string JobUid { get; set; }
        public string CJobInstanceUid { get; set; }
        public string BatchExecutionUid { get; set; }
    }
}