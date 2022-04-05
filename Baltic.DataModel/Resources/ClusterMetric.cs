using System.Collections.Generic;
using Baltic.DataModel.Execution;

namespace Baltic.DataModel.Resources
{
    public class ClusterMetric
    {
        public int SummaryValue { get; set; }
        public Benchmark Benchmark { get; set; }
        public List<TaskExecution> Results { get; set; }
    }
}