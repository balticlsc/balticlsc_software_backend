
namespace Baltic.UnitRegistry.Entities
{
    public class ComputationModuleEntity
    {
        // dziedziczone pola
        
        public int Id { get; set; }
        public string UId { get; set; }
        public string Name { get; set; }
        public int ProblemClassId { get; set; }// pclass : ProblemClass
        public int UserAccountId { get; set; }// Author : UserAccount, wynika z relacji (NOT NULL)
        public int? ComputationModuleId { get; set; } // ForkParent : ComputationUnit , wynika z relacji
    }
}