using System.Collections.Generic;
using Baltic.DataModel.CALExecutable;
namespace Baltic.DataModel.Execution {
	public abstract class JobInstance {
		public string InstanceUid { get; set; }
		public bool Completed { get; set; }
		public List<ResourceUsage> Usage { get; set; }
		public CJob Job { get; set; }
		public BatchExecution BatchExecution { get; set; }
		public List<JobExecution> Executions { get; set; }
		public JobExecution CurrentExecution { get; set; }
	}
}