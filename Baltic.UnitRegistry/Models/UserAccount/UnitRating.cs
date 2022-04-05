namespace UnitManager.DTO.ComputationAccounts
{
    public class UnitRating 
    {
        public int Value { get; set; }
        public string Comment { get; set; }

        public UserAccount  User { get; set; }
    }
}