using System;

namespace Baltic.TaskRegistry.Entities
{
    public class CustomInvariantValueEntity
    {
        public int Id { get; set; }
        public int TaskExecutionId { get; set; }
        public DateTime Stamp { get; set; }

        public string InvariantUid { get; set; }
        public string Value { get; set; }
    }
}
