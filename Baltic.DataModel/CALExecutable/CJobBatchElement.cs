using System.Collections.Generic;

namespace Baltic.DataModel.CALExecutable {
	public abstract partial class CJobBatchElement : CExecutable {
		public string ModuleReleaseUid { get; set; }
		public string Image { get; set; }
		public string Command { get; set; }
		public List<string> CommandArguments { get; set; }
		public List<CParameter> Parameters { get; set; }
	}
}