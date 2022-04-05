using System.Collections.Generic;

namespace Baltic.UnitRegistry.Models 
{
    public class PinGroup 
    {
        public string Name { get; set; }
        public IList<int> Depths { get; set; }
        public PinGroup()
        {
            Depths = new List<int>();
        }

    }
}