using System;

namespace Baltic.Security.Entities
{
    public class UserExtensionEntity
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }
        public string RoleInOrganisation { get; set; }
        public string Phone { get; set; }
        public string Address { get; set; }
        public string Country { get; set; }
        public string Language { get; set; }
        public DateTime Stamp { get; set; }
    }
}