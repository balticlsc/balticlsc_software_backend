namespace Baltic.DataModel.CALMessages {
	public class ResourceRequest {
		public int Cpus { get; set; }
		public int Memory { get; set; }
		public GpuRequest Gpus { get; set; }
	}
}