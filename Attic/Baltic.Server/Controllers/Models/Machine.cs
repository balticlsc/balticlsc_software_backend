using System.Collections.Generic;

namespace Baltic.Server.Controllers.Models
{
    public class Machine
    {
        public string Id { get; set; }
        public string Endpoint { get; set; }
        public string ResourceDescription { get; set; }
        public IEnumerable<Benchmark> BenchmarkManifest { get; set; }
        public IEnumerable<TimeSlot> AvailabilityManifest { get; set; }
    }
}
