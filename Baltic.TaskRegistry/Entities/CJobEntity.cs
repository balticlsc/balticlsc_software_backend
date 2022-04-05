using System;

namespace Baltic.TaskRegistry.Entities
{
    public class CJobEntity
    {
        public int Id { get; set; }
        public int CJobBatchId { get; set; }
        public DateTime Stamp { get; set; }

        public string Uid { get; set; }
        public string UnitUid { get; set; }
        public int Multiplicity { get; set; }
        public string BatchUid { get; set; }
        public string CallName { get; set; }
        public string YAML { get; set; }
    }
}
