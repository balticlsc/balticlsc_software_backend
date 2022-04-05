using Baltic.UnitRegistry.Models.Types;


namespace Baltic.UnitRegistry.Models 
{
    public class DeclaredDataPin  : DataPin 
    {
        public override string Name { get; set; }
        public override DataBinding Binding { get; set; }
        public override DataMultiplicity Multiplicity { get; set; }
        public override DataType Type { get; set; }
        public override AccessType Access { get; set; }

    }
}