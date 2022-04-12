using System.Collections.Generic;
using Baltic.DataModel.CALMessages;
using Baltic.DataModel.Execution;

namespace Baltic.DataModel.CALExecutable
{
    public abstract class CJob : CJobBatchElement
    {
        public int Multiplicity { get; set; }
        public string CallName { get; set; }
        public bool IsMultitasking { get; set; }
        public abstract CJobBatch Batch { get; set; }
        public abstract List<JobExecution> JobExecutions { get; set; }
        public abstract List<JobInstance> JobInstances { get; set; }
        public abstract BalticModuleBuild GetBuild(string pinsConfigMountPath);

    }
}