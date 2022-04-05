using System;

namespace Baltic.CalEditorRegistry.Entities
{
    public class ElementStyleEntity
    {
        public int Id { get; set; }
        public int ElementId { get; set; }
        public string Fill { get; set; }
        public int Opacity { get; set; }
        public string Stroke { get; set; }
        public int StrokeWidth { get; set; }
        public string Shape { get; set; }
        public bool PerfectDrawEnabled { get; set; }
        public int[] Dash { get; set; }
        public DateTime TimeStamp { set; get; } = DateTime.Now;
    }
}