namespace Baltic.UnitRegistry.Models 
{
    public class InvariantValue 
    {
        public string JSONDefault { get; set; }
        public bool UserChangeable { get; set; }

        public ExecInvariant  InvarDeclaration { get; set; }
    }
}