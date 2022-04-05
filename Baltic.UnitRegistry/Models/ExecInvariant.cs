namespace Baltic.UnitRegistry.Models 
{
    public class ExecInvariant 
    {
        public string UId { get; set; }
        public string Name { get; set; }
        public DataType  Type { get; set; }
        public ExecInvariant  Targetvariant { get; set; }
    }
}