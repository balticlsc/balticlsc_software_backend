using System;

namespace Baltic.TaskRegistry.Entities
{
    public class CTaskEntity
    {
        public int Id { get; set; }
        public DateTime Stamp { get; set; }

        public string Uid { get; set; }
        public string AppUid { get; set; }
    }
}
