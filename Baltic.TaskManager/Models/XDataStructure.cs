using System;

namespace Baltic.TaskManager.Models {
	public class XDataStructure : IComparable<XDataStructure> {
		public string Uid { get; set; }
		public string Name { get; set; }
		public string Version { get; set; }
		public bool IsBuiltIn { get; set; }
		public string DataSchema { get; set; }
		
		public int CompareTo(XDataStructure other)
		{
			return String.Compare(Name, other?.Name, StringComparison.Ordinal);
		}
	}
}