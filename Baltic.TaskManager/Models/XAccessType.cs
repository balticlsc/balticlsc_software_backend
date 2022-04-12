using System;

namespace Baltic.TaskManager.Models {
	public class XAccessType : IComparable<XAccessType> {
		public string Uid { get; set; }
		public string Name { get; set; }
		public string Version { get; set; }
		public int IsBuiltIn { get; set; }
		public string AccessSchema { get; set; }
		public string PathSchema { get; set; }
		
		public int CompareTo(XAccessType other)
		{
			return String.Compare(Name, other?.Name, StringComparison.Ordinal);
		}
	}
}