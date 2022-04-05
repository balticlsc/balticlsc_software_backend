using System.Collections.Generic;
using Baltic.UnitRegistry.Models.Types;


namespace Baltic.UnitRegistry.Models 
{
    public class UnitCall 
    {
        public string Name { get; set; }
        public UnitStrength Strength { get; set; }
        public List<InvariantValue > InvariantValues { get; set; }
        public ComputationUnitRelease  Unit { get; set; }
        public List<ComputedDataPin > Pins { get; set; }
        public UnitCall()
        {
            Pins = new List<ComputedDataPin>();
            InvariantValues = new List<InvariantValue>();
        }


    }
}