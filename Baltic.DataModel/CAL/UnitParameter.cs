using Baltic.DataModel.Types;

namespace Baltic.DataModel.CAL
{
    public class UnitParameter
    {
        public string NameOrPath { get; set; }
        public string DefaultValue { get; set; }
        public UnitParamType Type { get; set; }
        public bool IsMandatory { get; set; }
        public string Uid { get; set; }
        public string TargetParameterUid { get; set; }
    }
}