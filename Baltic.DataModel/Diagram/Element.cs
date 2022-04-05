using System.Collections.Generic;

namespace Baltic.DataModel.Diagram {
	public abstract class Element {
		public string Uid { get; set; }
		public List<Compartment> Compartments { get; set; }
		public string ElementType { get; set; }

		public Element()
		{
			Compartments = new List<Compartment>();
		}
	}
}