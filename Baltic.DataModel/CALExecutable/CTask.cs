using System.Collections.Generic;
using Baltic.DataModel.Execution;

namespace Baltic.DataModel.CALExecutable
{
    public abstract class CTask : CExecutable
    {
        public string ReleaseUid { get; set; }
        public string OwnerUid { get; set; }
        public abstract List<CJobBatch> Batches { get; set; }
        public abstract TaskExecution Execution { get; set; }
    }
}