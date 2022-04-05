
namespace Baltic.UnitRegistry.Entities
{
    public class ComputationApplicationReleaseEntity
    {
        //dziedziczone pola
        public int Id { get; set; }
        public string UId { get; set; }
        public string Name { get; set; }
        public string Version { get; set; }
        public int Status { get; set; } // ComputationUnitStatus enum
        
        public int ComputationApplicationId { get; set; } // Unit
        public int ? ListComputationApplicationId { get; set; } // Releases : List<ComputationUnitRelease>
    }
}