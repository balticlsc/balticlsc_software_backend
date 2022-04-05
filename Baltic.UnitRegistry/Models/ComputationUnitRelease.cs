using System.Collections.Generic;

using Baltic.UnitRegistry.Models.Inne;
using Baltic.UnitRegistry.Models.Types;
using UnitManager.DTO.ComputationAccounts;


namespace Baltic.UnitRegistry.Models 
{
    public abstract class ComputationUnitRelease 
    {
        public string UId { get; set; }
        public string Version { get; set; }
        public string Name => Unit.Name + " " + Version;
        public ComputationUnitStatus Status { get; set; }
        public ComputationUnit  Unit { get; set; }
        public ReleaseDescriptor  Descriptor { get; set; }
        public List<DeclaredDataPin> DeclaredDataPins { get; set; }
        public List<ExecInvariant> Invariants { get; set; }

        public ResourceReservationRange SupportedResourcesRange { get; set; }

        public ComputationUnitRelease()
        {
            // ! DeclaredPins = new List<DeclaredDataPin>();
            Invariants = new List<ExecInvariant>();
            Status = ComputationUnitStatus.Created;
            SupportedResourcesRange = new ResourceReservationRange();
        }

    }
}