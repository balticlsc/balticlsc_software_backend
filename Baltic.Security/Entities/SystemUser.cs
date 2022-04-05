using Baltic.Security.Auth;

namespace Baltic.Security.Entities
{
    public class SystemUser
    {
        public int Id { get; set; }        
        public string UserName { get; set; }
        public string EmailAddress { get; set; }
        public AccountStatusLookup Status { get; set; }
    }
}