using System;
using Baltic.DataModel.Types;

namespace Baltic.DataModel.Execution {
	public class ResourceUsage {

		public DateTime TimeStamp { get; set; }
		public ResourceKind Kind { get; set; }
		public float Value { get; set; }

	}
}