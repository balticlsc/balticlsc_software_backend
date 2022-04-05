
namespace Baltic.UnitRegistry.Entities
{
    #nullable enable
    public class ComputationApplicationEntity
    {
        // Dziedziczone pola
        public int Id { get; set; }
        public string Name { get; set; }
        public string UId { get; set; }
        public int ProblemClassId { get; set; }// pclass : ProblemClass
        public int UserAccountId { get; set; }// Author : UserAccount, wynika z relacji
        public int ? ComputationApplicationId { get; set; } // ForkParent : ComputationUnit , wynika z relacji
        
    }
}