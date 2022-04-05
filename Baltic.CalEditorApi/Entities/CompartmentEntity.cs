using System;

namespace Baltic.CalEditorRegistry.Entities
{
    public class CompartmentEntity
    {
        public int _Id { get; set; }
        public int _ElementId { get; set; }
        public string CompartmentUuid { get; set; }
        public Guid ElementUuid { get; set; }
        public string Input { get; set; }
        public string Value { get; set; }
        public string CompartmentTypeId { get; set; }
        public DateTime TimeStamp { set; get; }
    }
}