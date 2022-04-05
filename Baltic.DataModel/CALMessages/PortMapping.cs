using Baltic.DataModel.Types;

namespace Baltic.DataModel.CALMessages {
	public class PortMapping {
		public uint ContainerPort { get; set; }
		public uint PublishedPort { get; set; }
		public CommProtocol Protocol { get; set; }
	}
}