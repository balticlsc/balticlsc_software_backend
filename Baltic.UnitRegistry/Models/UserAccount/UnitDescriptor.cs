using System.Collections.Generic;

namespace UnitManager.DTO.ComputationAccounts
{
    public class UnitDescriptor 
    {
        public string ShortDescription { get; set; }
        public string LongDescription { get; set; }
        public List<string> Keywords { get; set; }
        public string Icon { get; set; } 
        public IList<UnitRating> Ratings { get; set; }
    }
}