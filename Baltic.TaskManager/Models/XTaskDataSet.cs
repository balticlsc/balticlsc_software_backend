using Baltic.DataModel.Types;

namespace Baltic.TaskManager.Models
{
    public class XTaskDataSet
    {
        public string Uid { get; set; }
        public string Name { get; set; }
        
        public string DataTypeUid { get; set; }
        public string DataTypeName { get; set; }
        public string DataTypeVersion { get; set; }
        
        public CMultiplicity Multiplicity { get; set; }
        
        public string DataStructureUid { get; set; }
        public string DataStructureName { get; set; }
        public string DataStructureVersion { get; set; }
        
        public string AccessTypeUid { get; set; }
        public string AccessTypeName { get; set; }
		public string AccessTypeVersion { get; set; }
        
        public string Values { get; set; }
        public string AccessValues { get; set; }
    }
}