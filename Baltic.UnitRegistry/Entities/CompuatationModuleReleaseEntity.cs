
namespace Baltic.UnitRegistry.Entities
{
    public class ComputationModuleReleaseEntity
    {
        public string YAML { get; set; }
        public int ResourceReservationRangeId { get; set; } //SupportedResourcesRange, wynika z relacji

        // dziedzidzone pola
        public int Id { get; set; }
        public string UId { get; set; }
        public string Name { get; set; }
        public string Version { get; set; }
        public int Status { get; set; } // ComputationUnitStatus enum

        public int ComputationModuleId { get; set; } // ( ) Unit ,  wynika z asocjacji
        public int ? ListComputationModuleID { get; set; } // Releases : List<ComputationUnitRelease>
    }
}