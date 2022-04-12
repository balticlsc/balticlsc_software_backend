using System;

namespace Baltic.TaskManager.Models {
	public class XDataType : IComparable<XDataType> {
		public string Uid { get; set; }
		public string Name { get; set; }
		public string Version { get; set; }
		public bool IsBuiltIn { get; set; }
		public bool IsStructured { get; set; }
		public int CompareTo(XDataType other)
		{
			return String.Compare(Name, other?.Name, StringComparison.Ordinal);
		}
	}
}