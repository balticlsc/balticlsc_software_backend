using System;

namespace Baltic.TaskRegistry.Entities
{
    public class CJobBatchEntity
    {
        public int Id { get; set; }
        public int CTaskId { get; set; }
        public DateTime Stamp { get; set; }
        
        public string Uid { get; set; }
        public int DepthLevel { get; set; }
        public int SerialNo { get; set; }
    }
}
