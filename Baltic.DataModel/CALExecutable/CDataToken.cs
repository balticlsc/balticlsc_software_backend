using System.Collections.Generic;
using Baltic.DataModel.Types;

namespace Baltic.DataModel.CALExecutable
{
    public abstract class CDataToken {

        public string Uid { get; set; }
        public long TokenNo { get; set; }
        public string PinUid { get; set; }
        public string PinName { get; set; }
        public DataBinding Binding { get; set; }
        public CMultiplicity DataMultiplicity { get; set; }
        public CMultiplicity TokenMultiplicity { get; set; }
        public List<int> Depths { get; set; }
        public bool Direct { get; set; }
        public string DataType { get; set; }
        public string AccessType { get; set; }
        public abstract CDataSet Data { get; set; }
        public abstract CDataSet AccessData { get; set; }
        public CService Service { get; set; }

        public CDataToken(){
            Depths = new List<int>();
        }
		
        public override string ToString() {
            string ret = "DataToken " + PinName + " " + PinUid + " " + TokenNo + " " + Binding + " " + TokenMultiplicity +
                         " dir=" + Direct + " acc=" + AccessType;
            foreach (int i in Depths)
                ret += (0 == Depths.IndexOf(i) ? " #depths: " : ",") + i;
            return ret + "\n";
        }

    }
}