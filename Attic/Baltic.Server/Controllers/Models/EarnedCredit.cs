using System.Collections.Generic;

namespace Baltic.Server.Controllers.Models
{
    public class EarnedCredit
    {
        public IEnumerable<string> Intervals { get; set; }
        public IEnumerable<float> Credits { get; set; }
    }
}
