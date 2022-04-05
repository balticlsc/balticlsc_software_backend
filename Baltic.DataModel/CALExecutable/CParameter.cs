using Baltic.DataModel.CAL;
using Baltic.DataModel.Types;

namespace Baltic.DataModel.CALExecutable {
	public class CParameter {
		public string NameOrPath { get; set; }
		public string Value { get; set; }
		public UnitParamType Type { get; set; }
		
		public CParameter(UnitParameter source)
		{
			NameOrPath = source.NameOrPath;
			Value = source.DefaultValue;
			Type = source.Type;
		}
		
		public CParameter(CParameter source = null)
		{
			if (null == source)
				return;
			NameOrPath = source.NameOrPath;
			Value = source.Value;
			Type = source.Type;
		}
	}
}