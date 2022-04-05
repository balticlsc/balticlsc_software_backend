
using Baltic.UnitRegistry.Models.Types;

namespace Baltic.UnitRegistry.Models 
{
    public abstract class DataPin 
    {
        public abstract string Name { get; set; }
        public abstract DataBinding Binding { get; set; }
        public abstract DataMultiplicity Multiplicity { get; set; }
        public abstract DataType Type { get; set; }
        public abstract AccessType Access { get; set; }

        public DataFlow  Incoming { get; set; }
        public DataFlow  Outgoing { get; set; }
    }
}