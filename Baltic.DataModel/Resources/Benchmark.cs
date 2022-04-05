using Baltic.DataModel.CAL;
using Baltic.DataModel.Types;

namespace Baltic.DataModel.Resources
{
    public class Benchmark
    {
        public string Name { get; set; }
        public BenchmarkType Type { get; set; }
        public ComputationUnitRelease Unit { get; set; }
    }
}