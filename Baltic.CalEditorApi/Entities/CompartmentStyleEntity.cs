using System;

namespace Baltic.CalEditorRegistry.Entities
{
    public class CompartmentStyleEntity
    {
        public int Id { get; set; }
        public int CompartmentId { get; set; }
        public string Align { get; set; }
        public string Fill { get; set; }
        public string FontFamily { get; set; }
        public string FontSize { get; set; }
        public string FontStyle { get; set; }
        public string FontVariant { get; set; }
        public string StrokeWidth { get; set; }
        public bool Visible { get; set; }
        public int Y { get; set; }
        public string Text { get; set; }
        public bool Listening { get; set; }
        public bool PerfectDrawEnabled { get; set; }
        public string Width { get; set; }
        public string Height { get; set; }
        public int Padding { get; set; }
        public string Placement { get; set; }
        public DateTime TimeStamp { set; get; } = DateTime.Now;
    }
}