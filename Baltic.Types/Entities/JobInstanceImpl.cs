using Baltic.DataModel.Execution;

namespace Baltic.Types.Entities
{
    public class JobInstanceImpl : JobInstance
    {
        public string JobUid { get; set; }
        public string BatchExecutionUid { get; set; }
    }
}