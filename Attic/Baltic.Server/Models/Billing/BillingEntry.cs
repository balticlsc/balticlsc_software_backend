namespace Baltic.Server.Models.Billing
{
    public class BillingEntry
    {
        public string Id { get; set; }
        public string Type { get; set; }
        public string Direction { get; set; } //direction: in , out, in/out
        public string Value { get; set; }
    }
}
