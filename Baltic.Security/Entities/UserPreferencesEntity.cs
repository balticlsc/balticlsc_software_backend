using System;

namespace Baltic.Security.Entities
{
    public class UserPreferencesEntity
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string Param { get; set; }
        public string Value { get; set; }
        public DateTime Stamp { get; set; }        
    }
}