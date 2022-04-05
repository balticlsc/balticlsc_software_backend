using System;

namespace Baltic.Security.Entities
{
    public class UserRolesEntity
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int Role { get; set; }
        public DateTime Stamp { get; set; } 
    }
}