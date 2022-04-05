namespace Baltic.DataModel.CALMessages {
	public class VolumeDescription {
		public int Size { get; set; }
		public string StorageClass { get; set; }
		public string MountPath { get; set; }

		public VolumeDescription()
		{
			StorageClass = "";
			MountPath = "";
		}
	}
}