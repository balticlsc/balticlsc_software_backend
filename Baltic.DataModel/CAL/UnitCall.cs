using System.Collections.Generic;
using Baltic.DataModel.Types;

namespace Baltic.DataModel.CAL
{
    public class UnitCall
    {
        public string Name { get; set; }
        public UnitStrength Strength { get; set; }
        public ComputationUnitRelease Unit { get; set; }
        public List<ComputedDataPin> Pins { get; set; }
        public List<UnitParameterValue> ParameterValues { get; set; }

        public UnitCall()
        {
            Pins = new List<ComputedDataPin>();
            ParameterValues = new List<UnitParameterValue>();
        }

    }
}