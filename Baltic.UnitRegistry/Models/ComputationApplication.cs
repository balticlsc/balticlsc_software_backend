using System.Collections.Generic;
using Baltic.UnitRegistry.Models.Inne;


namespace Baltic.UnitRegistry.Models
{
    public class ComputationApplication  : ComputationUnit 
    {
        public CALDiagram  Diagram { get; set; }
        public override List<ComputationUnitRelease> Releases { get; set; }
    }
}