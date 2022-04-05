using Baltic.DataModel.Types;

namespace Baltic.DataModel.CAL
{
    public class ComputedDataPin : DataPin
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
        
        public override DataType Type
        {
            get => Declared.Type;
            set => Declared.Type = value;
        }

        public override DataStructure Structure
        {
            get => Declared.Structure;
            set => Declared.Structure = value;
        }

        public override CMultiplicity DataMultiplicity
        {
            get => Declared.DataMultiplicity; 
            set => Declared.DataMultiplicity = value;
        }

        public override CMultiplicity TokenMultiplicity
        {
            get => Declared.TokenMultiplicity;
            set => Declared.TokenMultiplicity = value;
        }
        
        public override AccessType Access
        {
            get => Declared.Access;
            set => Declared.Access = value;
        }

        public DeclaredDataPin Declared  { get; set; }
        public UnitCall Call { get; set; }
        public PinGroup Group  { get; set; }
    }
}