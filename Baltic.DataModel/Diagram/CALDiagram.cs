using System.Collections.Generic;

namespace Baltic.DataModel.Diagram
{
	public class CALDiagram {
		public string Uid { get; set; }
		public string Name { get; set; }
		public List<Port> Ports { get; set; }
		public List<Box> Boxes { get; set; }
		public List<Line> Lines { get; set; }
	}
}