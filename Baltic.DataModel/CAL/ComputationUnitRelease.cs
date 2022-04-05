using System.Collections.Generic;
using Baltic.DataModel.Accounts;
using Baltic.DataModel.Execution;
using Baltic.DataModel.Types;

namespace Baltic.DataModel.CAL
{
    public abstract class ComputationUnitRelease
    {
        public string Version { get; set; }
        public string Uid { get; set; }
        public UnitReleaseStatus Status { get; set; }
        public List<DeclaredDataPin> DeclaredPins { get; set; }
        public ComputationUnit Unit { get; set; }
        public ReleaseDescriptor Descriptor { get; set; }
        public List<UnitParameter> Parameters { get; set; }
        public ResourceReservationRange SupportedResourcesRange { get; set; }


        public string Name => Unit.Name + " " + Version;

        public ComputationUnitRelease()
        {
            DeclaredPins = new List<DeclaredDataPin>();
            Parameters = new List<UnitParameter>();
            Status = UnitReleaseStatus.Created;
        }
    }
}