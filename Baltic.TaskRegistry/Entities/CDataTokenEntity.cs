using System;

namespace Baltic.TaskRegistry.Entities
{
    public class CDataTokenEntity
    {
        public int Id { get; set; }
        public DateTime Stamp { get; set; }
        public int? ContainingEntityId { get; set; } // CTaskId / CJobBatchId / CJobId
        public int CDataSetId { get; set; }

        public string Uid { get; set; }
        public long TokenNo { get; set; }
        public string PinUid { get; set; }
        public string PinName { get; set; }
        public string AccessType { get; set; }  //czy to jest rzeczywiście string, czy jednak AccesType?
        public int Binding { get; set; }        //(enum: DataBinding)
        public int Multiplicity { get; set; }   //(enum: CMultiplicity)
        public string DepthsJSON { get; set; } //(serialized list: Depths List<int>)
        public bool Direct { get; set; }
    }
}
