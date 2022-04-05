using System.Collections.Generic;
using Baltic.UnitRegistry.Models.Inne;

namespace Baltic.UnitRegistry.Models 
{
    public class ComputationApplicationRelease  : ComputationUnitRelease 
    {
        public CALDiagram  Diagram { get; set; }
        public IList<UnitCall> Calls { get; set; }
        public IList<DataFlow> Flows { get; set; }
        
        public ComputationApplicationRelease()
        {
            Calls = new List<UnitCall>();
            Flows = new List<DataFlow>();
        }

    }
}