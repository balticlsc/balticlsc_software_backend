using System.Linq;
using System.Security.Claims;

namespace Baltic.Security.Auth
{
    public static class ClaimsPrincipalExtensions
    {
        public static bool IsInPolicy(this ClaimsPrincipal claimsPrincipal, PoliciesLookup.Policy policy)
            => policy.AllowedRoles.Any(claimsPrincipal.IsInRole);
    }
}