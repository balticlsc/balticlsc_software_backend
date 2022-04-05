using System.Collections.Generic;

namespace Baltic.CalEditorRegistry.Model
{
    public class Element
    {
        public string Id { get; set; }
        public string ElementTypeId { get; set; }
        public string DiagramId { get; set; }
        public string Data { get; set; }
        public DiagramElementType Type { get; set; }
        public List<Compartment> Compartments { get; set; } = new List<Compartment>();
    }
}