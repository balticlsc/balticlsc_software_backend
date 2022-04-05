using System.Collections.Generic;

namespace Baltic.UnitRegistry.Models 
{
    public class ComputationModule  : ComputationUnit 
    {
        public override List<ComputationUnitRelease> Releases { get; set; }
    }
}