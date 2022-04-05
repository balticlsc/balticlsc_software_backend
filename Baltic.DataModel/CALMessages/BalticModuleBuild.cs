using System.Collections.Generic;
using Baltic.DataModel.Types;

namespace Baltic.DataModel.CALMessages {
	public partial class BalticModuleBuild
	{
		public string BatchId { get; set; }
		public string ModuleId { get; set; }
		public string Image { get; set; }
		public Dictionary<string,string> EnvironmentVariables  { get; set; }
		public string Command  { get; set; }
		public List<string> CommandArguments  { get; set; }
		public List<PortMapping> PortMappings  { get; set; }
		public List<VolumeDescription> Volumes  { get; set; }
		public ResourceRequest Resources  { get; set; }
		public List<ConfigFileDescription> ConfigFiles { get; set; }
		public NetworkScope Scope { get; set; }
	}
}