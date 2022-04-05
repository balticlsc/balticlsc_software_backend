using System.Collections.Generic;
using Baltic.DataModel.Types;

namespace Baltic.TaskManager.Models {
	public class XDataToken {
        public string Uid { get; set; }
        public long TokenNo { get; set; }
        public string PinUid { get; set; }
        public string PinName { get; set; }
        public string AccessType { get; set; }
        public DataBinding Binding { get; set; }
        public CMultiplicity Multiplicity { get; set; }
        public List<int> Depths { get; set; }
        public bool Direct { get; set; }
        public XDataSet XDataSet { get; set; }
    }
}