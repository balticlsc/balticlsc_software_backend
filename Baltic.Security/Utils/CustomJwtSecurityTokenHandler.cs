using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using Baltic.Security.Auth;
using Microsoft.IdentityModel.Tokens;

namespace Baltic.Security.Utils
{
    public class CustomJwtSecurityTokenHandler : ISecurityTokenValidator
    {
        private int _maxTokenSizeInBytes = TokenValidationParameters.DefaultMaximumTokenSizeInBytes;
        private readonly bool _checkSession;
        private readonly JwtSecurityTokenHandler _tokenHandler;
        private readonly UserRegistryRepository _userRegistry = new UserRegistryRepository();

        public CustomJwtSecurityTokenHandler(bool checkSession = true)
        {
            _tokenHandler = new JwtSecurityTokenHandler();
            _checkSession = checkSession;
        }

        public bool CanValidateToken
        {
            get
            {
                return true;
            }
        }

        public int MaximumTokenSizeInBytes
        {
            get
            {
                return _maxTokenSizeInBytes;
            }

            set
            {
                _maxTokenSizeInBytes = value;
            }
        }

        public bool CanReadToken(string securityToken)
        {
            return _tokenHandler.CanReadToken(securityToken);            
        }

        public ClaimsPrincipal ValidateToken(string securityToken, TokenValidationParameters validationParameters, out SecurityToken validatedToken)
        {
            var principal = _tokenHandler.ValidateToken(securityToken, validationParameters, out validatedToken);
            
            var sid = principal.Claims.Where(c => c.Type == JwtRegisteredClaimNames.Sid).Select(c => c.Value).SingleOrDefault();
            var jti = principal.Claims.Where(c => c.Type == JwtRegisteredClaimNames.Jti).Select(c => c.Value).SingleOrDefault();
            
            var session = _userRegistry.GetSession(sid);

            var identity = new ClaimsIdentity(principal.Identity);
            identity.AddClaim(new Claim(ClaimTypes.Role, RoleLookup.Supervisor));
            
            if (_checkSession)
            {
                if (session != null && session.Jti == jti)
                {
                    return new ClaimsPrincipal(identity);
                }
                throw new SecurityTokenValidationException("Invalid session, please login again");
            }
            return new ClaimsPrincipal(identity);
        }
    }
}