using System.Collections.Generic;

namespace Baltic.DataModel.Types
{
    public class DataType : CalType
    {
		public bool IsStructured { get; set; }
		public List<string> CompatibleAccessTypeUids { get; set; }
		public string ParentUid { get; set; }

		public DataType()
		{
			CompatibleAccessTypeUids = new List<string>();
		}
    }
}