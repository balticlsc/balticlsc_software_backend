using System.Collections.Generic;

namespace Baltic.Server.Controllers.Models
{
    public class Node
    {
        public string Id { get; set; }
        public string Status { get; set; } // {active } 
        public string Endpoint { get; set; }
        public bool IsPrivate { get; set; }
        public string Country { get; set; }
        public IEnumerable<Benchmark> Performance { get; set; }
        public IEnumerable<string> DataThroughput { get; set; } // GB/s
        public IEnumerable<TimeSlot> TimeAvailability { get; set; }
        public short HighAvailability { get; set; }

        //TODO wskazanie na głównego i zapasowe MasterNode
    }
}
