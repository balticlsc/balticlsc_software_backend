using Baltic.DataModel.Types;

namespace Baltic.Node.BatchManager.Models
{
	public class XJobStatus
	{
		public ComputationStatus Status { get; set; }
		public long JobProgress { get; set; }
	}
}