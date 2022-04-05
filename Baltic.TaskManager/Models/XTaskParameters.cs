using System.Collections.Generic;
using Baltic.Types.Models;

namespace Baltic.TaskManager.Models {
	public class XTaskParameters : XReservationRange{
		public string TaskName { get; set; }
		public int Priority { get; set; }
		public string ClusterAllocation { get; set; }
		public string ClusterUid { get; set; }
		public float ReservedCredits { get; set; }
		public float AuxStorageCredits { get; set; }
		public bool IsPrivate { get; set; }
		public IEnumerable<XCustomInvariantValue> Invariants { get; set; }
	}
}