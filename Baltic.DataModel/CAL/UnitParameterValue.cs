namespace Baltic.DataModel.CAL
{
    public class UnitParameterValue
    {
        public string Value { get; set; }
        public bool UserChangeable { get; set; }
        public UnitParameter Declaration { get; set; }
    }
}