using System;

namespace Baltic.CalEditorRegistry.Entities
{
    public class DiagramEntity
    {
        public int _Id { get; set; }
        public Guid DiagramUuid { get; set; }
        public string Name { get; set; }
        public string Data { get; set; }
        public DateTime TimeStamp { set; get; }
    }
}