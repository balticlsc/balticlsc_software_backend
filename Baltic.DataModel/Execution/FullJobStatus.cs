namespace Baltic.DataModel.Execution {
	public class FullJobStatus : JobStatus
	{
		public long TokensReceived { get; set; }
		public long TokensProcessed { get; set; }
	}
}