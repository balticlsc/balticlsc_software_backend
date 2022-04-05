using Baltic.DataModel.Types;

namespace Baltic.DataModel.CAL
{
    public abstract class DataPin
    {
        public string Uid { get; set; }
        public long TokenNo;

        public abstract string Name { get; set; }
        public abstract DataBinding Binding { get; set; }
        public abstract DataType Type { get; set; }
        public abstract DataStructure Structure { get; set; }
        public abstract CMultiplicity DataMultiplicity { get; set; }
        public abstract CMultiplicity TokenMultiplicity { get; set; }
        public abstract AccessType Access { get; set; }

        public bool IsDirect => null == Access || string.IsNullOrEmpty(Access.AccessSchema);

        public DataFlow Incoming { get; set; }
        public DataFlow Outgoing { get; set; }
    }
}