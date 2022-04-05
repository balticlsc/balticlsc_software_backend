namespace Baltic.DataModel.Types {
	public abstract class CalType {
		public string Name { get; set; }
		public string Version { get; set; }
		public bool IsBuiltIn { get; set; }
		public string Uid { get; set; }
	}
}