namespace UnitManager.Entity.ComputationAccounts
{
    public class AppShelfEntity
    {
        public int Id { get; set; }
        public int ? OwnerUserAccountId { get; set; } // Owner : UserAccount, wynika z relacji
        public int ComputationApplicationReleaseId { get; set; }
        public int ComputationModuleReleaseId { get; set; }

    }
    public class ToolBoxEntity
    {
        public int Id { get; set; }
        public int? DeveloperUserAccountId { get; set; } // Developer :UserAccount, wynika z relacji
        public int ComputationApplicationReleaseId { get; set; }
        public int ComputationModuleReleaseId { get; set; }
    }
}