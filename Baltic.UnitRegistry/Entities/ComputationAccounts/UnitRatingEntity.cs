namespace UnitManager.Entity.ComputationAccounts
{
    public class UnitRatingEntity
    {
        public int Id { get; set; }
        public int Value { get; set; }
        public string Comment { get; set; }

        public int ? UnitDescriptorId { get; set; } // Ratings : UnitRating, wunika z agregacji
        public int UserAccountId { get; set; } //User: UserAccount, wynika z  relacji
    }
}