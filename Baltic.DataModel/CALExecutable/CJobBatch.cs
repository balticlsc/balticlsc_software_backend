using System.Collections.Generic;
using Baltic.DataModel.Execution;

namespace Baltic.DataModel.CALExecutable
{
    public abstract class CJobBatch : CExecutable
    {
        public int DepthLevel { get; set; }
        public int SerialNo { get; set; }
        
        public CTask Task { get; set; }
        public abstract List<CJob> Jobs { get; set; }
        public abstract List<CService> Services { get; set; }
        public abstract List<BatchExecution> BatchExecutions { get; set; }
        public ResourceReservationRange DerivedReservationRange { get; set; }

        public CJobBatch()
        {
            DepthLevel = -1;
        }

    }
}