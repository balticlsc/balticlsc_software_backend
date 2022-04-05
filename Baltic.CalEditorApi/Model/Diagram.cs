using System.Collections.Generic;

namespace Baltic.CalEditorRegistry.Model
{
    public class Diagram
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Data { get; set; }
        public List<Box> Boxes { get; set; } = new List<Box>();
        public List<Line> Lines { get; set; } = new List<Line>();
        public List<Port> Ports { get; set; } = new List<Port>();
    }
}