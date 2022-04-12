using System;

#pragma warning disable 1591
namespace Baltic.Server.Controllers.Models
{
    [Obsolete]
    public class ApplicationRequest
    {
        public string Id { get; set; }
        public string ComputationApplicationId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string UserId { get; set; }
        public bool Notify { get; set; }
    }
}
