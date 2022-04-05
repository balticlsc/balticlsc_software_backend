using System;

namespace Baltic.CalEditorRegistry.Entities
{
    public class Element
    {
        public int _Id { get; set; }
        public int _DiagramId { get; set; }
        public Guid ElementUuid { get; set; }
        public Guid DiagramUuid { get; set; }
        public string ElementTypeId { get; set; }
        public string Data { get; set; }
        public DateTime TimeStamp { set; get; }
    }
}