using System;

namespace Baltic.CalEditorRegistry.Entities
{
    public class LocationEntity
    {
        public int Id { get; set; }
        public int ElementId { get; set; }
        public int Height { get; set; }
        public int Width { get; set; }
        public int X { get; set; }
        public int Y { get; set; }
        public DateTime TimeStamp { set; get; } = DateTime.Now;
    }
}