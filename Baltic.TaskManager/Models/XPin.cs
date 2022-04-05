using Baltic.DataModel.Types;

namespace Baltic.TaskManager.Models
{
    public class XPin
    {
        public string PinUid { get; set; }

        public string PinName { get; set; }

        public DataBinding Binding { get; set; }

        public CMultiplicity Multiplicity { get; set; }

        public string DataType { get; set; }
    }
}