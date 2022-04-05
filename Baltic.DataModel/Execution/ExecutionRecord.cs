using System;
using Baltic.DataModel.Types;

namespace Baltic.DataModel.Execution
{
    public abstract class ExecutionRecord
    {
        public ComputationStatus Status { get; set; }
        public DateTime Start { get; set; }
        public DateTime Finish { get; set; }
    }
}