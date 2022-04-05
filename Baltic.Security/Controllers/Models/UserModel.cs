using Baltic.Security.Auth;

namespace Baltic.Security.Controllers.Models
{
    public class UserModel
    {
        public string Username { get; set; }
        public string Password { get; set; }        
        public string EmailAddress { get; set; }
        public AccountStatusLookup Status { get; set; }
    }
}