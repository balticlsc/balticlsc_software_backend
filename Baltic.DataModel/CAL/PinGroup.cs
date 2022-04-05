using System.Collections.Generic;

namespace Baltic.DataModel.CAL
{
    public class PinGroup
    {
        public string Name;
        public List<int> Depths { get; set; }

        public PinGroup()
        {
            Depths = new List<int>();
        }

    }
}