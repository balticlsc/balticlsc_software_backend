namespace Baltic.DataModel.Accounts
{
    public class UnitRating
    {
        public int Value { get; set; }
        public string Comment { get; set; }
        public UserAccount User { get; set; }
    }
}