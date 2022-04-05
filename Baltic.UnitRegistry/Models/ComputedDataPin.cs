
using Baltic.UnitRegistry.Models.Types;

namespace Baltic.UnitRegistry.Models 
{
    public class ComputedDataPin  : DataPin 

    {
        public override string Name
        {
            get => Declared.Name;
            set => Declared.Name = value;
        }

        public override DataBinding Binding
        {
            get => Declared.Binding;
            set => Declared.Binding = value;
        }

        public override DataMultiplicity Multiplicity
        {
            get => Declared.Multiplicity;
            set => Declared.Multiplicity = value;
        }

        public override DataType Type
        {
            get => Declared.Type;
            set => Declared.Type = value;
        }

        public override AccessType Access
        {
            get => Declared.Access;
            set => Declared.Access = value;
        }



        public DeclaredDataPin  Declared { get; set; }

        public PinGroup  Group { get; set; }
    }
}