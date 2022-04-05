using System;

namespace Baltic.TaskRegistry.Entities
{
    public class ResourceReservationEntity
    {
        public int Id { get; set; }
        public DateTime Stamp { get; set; }

        public int CPUs { get; set; }
        public int GPUs { get; set; }
        public int Memory { get; set; }
        public int Storage { get; set; }
    }
}
