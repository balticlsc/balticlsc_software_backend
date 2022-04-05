using System.Collections.Generic;

namespace Baltic.DataModel.CAL
{
    public class ComputationApplicationRelease : ComputationUnitRelease
    {
        public string DiagramUid { get; set; }
        public List<UnitCall> Calls { get; set; }
        public List<DataFlow> Flows { get; set; }
        public ComputationApplicationRelease()
        {
            Calls = new List<UnitCall>();
            Flows = new List<DataFlow>();
        }
    }
}