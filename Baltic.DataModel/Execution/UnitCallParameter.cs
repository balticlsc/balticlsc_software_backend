using Baltic.DataModel.CAL;
using Baltic.DataModel.CALExecutable;

namespace Baltic.DataModel.Execution
{
    public class UnitCallParameter : CParameter
    {
        public string InvariantUid { get; set; } // TODO - remove
        public UnitParameter Declaration { get; set; } = null;

    }
}