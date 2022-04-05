using System.Collections.Immutable;
using System.Linq;
using System.Security.Claims;

namespace Baltic.Security.Auth
{
    public static class IdentityExtensions
    {
        public static ImmutableList<string> GetRoles(this ClaimsPrincipal claimsPrincipal)
            => claimsPrincipal.Claims
                .Where(claim => claim.Type == ClaimTypes.Role)
                .Select(claim => claim.Value)
                .ToImmutableList();
    }
}