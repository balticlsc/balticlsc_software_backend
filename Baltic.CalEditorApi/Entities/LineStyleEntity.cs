using System;

namespace Baltic.CalEditorRegistry.Entities
{
    public class LineStyleEntity
    {
        public int Id { get; set; }
        public int LineId { get; set; }
        public string LineType { get; set; }
        public DateTime TimeStamp { set; get; } = DateTime.Now;
    }
}