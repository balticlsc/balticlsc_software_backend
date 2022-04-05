using System.Collections.Generic;

namespace Baltic.DataModel.Diagram {
	public class ConnectableElement : Element {
		public Line Outgoing { get; set; }
		public Line Incoming { get; set; }
		public Dictionary<string,string> Data { get; set; }

		public ConnectableElement(){
			Data = new Dictionary<string, string>();
		}
	}
}