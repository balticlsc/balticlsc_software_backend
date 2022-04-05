using System.Collections.Generic;
using Baltic.DataModel.Accounts;
using Baltic.DataModel.Types;

namespace Baltic.DataModel.CAL
{
    public abstract class ComputationUnit
    {
        public string Name { get; set; }
        public string Uid { get; set; }
        public ProblemClass Class { get; set; }
        public string ForkParentUid { get; set; }
        public UnitDescriptor Descriptor { get; set; }
        public string AuthorUid { get; set; }

        public List<ComputationUnitRelease> Releases { get; set; }

        public ComputationUnit()
        {
            Releases = new List<ComputationUnitRelease>();
        }
    }
}