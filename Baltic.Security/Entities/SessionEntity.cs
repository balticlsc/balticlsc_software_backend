using System;

namespace Baltic.Security.Entities
{
    public class SessionEntity
    {
        public int Id { set; get; }
        public int UserId { get; set; }
        public string Sid { get; set; }
        public string Jti { get; set; }
        public DateTime Stamp { set; get; }        
    }
}