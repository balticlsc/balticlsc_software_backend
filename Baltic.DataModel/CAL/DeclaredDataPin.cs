using Baltic.DataModel.Types;

namespace Baltic.DataModel.CAL
{
    public sealed class DeclaredDataPin : DataPin
    {
        public override string Name { get; set; }
        public override DataBinding Binding { get; set; }
        public override DataType Type { get; set; }
        public override DataStructure Structure { get; set; }
        public override CMultiplicity DataMultiplicity { get; set; }
        public override CMultiplicity TokenMultiplicity { get; set; }
        public override AccessType Access { get; set; }
        
        public DeclaredDataPin(DataPin pin = null)
        {
            if (null == pin)
                return;
            Uid = pin.Uid;
            TokenNo = pin.TokenNo;
            Name = pin.Name;
            Binding = pin.Binding;
            Type = pin.Type;
            Structure = pin.Structure;
            DataMultiplicity = pin.DataMultiplicity;
            TokenMultiplicity = pin.TokenMultiplicity;
            Access = pin.Access;
        }
    }
}