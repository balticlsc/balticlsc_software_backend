using System.Collections.Generic;

namespace Baltic.DataModel.CAL {
	public class CAppReleaseExecutableOld {
		public List<UnitCall> Calls { get; set; }
		public List<DataFlow> Flows { get; set; }

		public ComputationApplicationRelease Base { get; set; }
		public bool Outermost { get; set; }

		public CAppReleaseExecutableOld()
		{
			Calls = new List<UnitCall>();
			Flows = new List<DataFlow>();
		}

		public CAppReleaseExecutableOld(ComputationApplicationRelease release)
		{
			Calls = new List<UnitCall>();
			Flows = new List<DataFlow>();
			Base = release;
		}
	}
}