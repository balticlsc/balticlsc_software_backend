namespace UnitManager.Entity
{
    public class UnitDescriptorEntity
    {
        public int Id { get; set; }
        public string ShortDescription { get; set; }
        public string LongDescription { get; set; }
        public string Icon { get; set; } 
        public int ComputationUnitId { get; set; } // Descriptor : UnitDescriptor , wynika z kompozycji
    }
}