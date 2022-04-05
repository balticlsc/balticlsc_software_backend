using System.Collections.Generic;
using Baltic.DataModel.Execution;

namespace Baltic.DataModel.CALMessages
{
    public class BatchInstanceMessage : BatchExecutionMessage
    {
        public int DepthLevel { get; set; }
        public ResourceReservation Quota { get; set; }
        public List<BalticModuleBuild> ServiceBuilds { get; set; }

        public BatchInstanceMessage()
        {
            ServiceBuilds = new List<BalticModuleBuild>();
        }
    }
}