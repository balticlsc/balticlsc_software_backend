using System.Collections.Generic;
using UnitManager.DTO.ComputationAccounts;


namespace Baltic.UnitRegistry.Models 
{
    public abstract class ComputationUnit 
    {
        public string Name { get; set; }
        public string UId { get; set; }
        public abstract List<ComputationUnitRelease > Releases { get; set; }
        public ProblemClass  PClass { get; set; }
        public UserAccount  Author { get; set; }
        public ComputationUnit  ForkParent { get; set; }
        public UnitDescriptor  Descriptor { get; set; }
    }
}