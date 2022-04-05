namespace Baltic.DataModel.CALMessages {
	public class ConfigFileDescription {
		public string Data { get; set; }
		public string MountPath { get; set; }

		public ConfigFileDescription()
		{
			Data = "";
			MountPath = "";
		}

		public new string ToString()
		{
			return $"Path: {MountPath} Data:{Data}";
		}
	}
}