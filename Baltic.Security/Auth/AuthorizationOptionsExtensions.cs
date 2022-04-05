using Microsoft.AspNetCore.Authorization;

namespace Baltic.Security.Auth
{
    public static class AuthorizationOptionsExtensions
    {
        public static void AddPolicy(this AuthorizationOptions authOptions, PoliciesLookup.Policy policy)
        {
            authOptions.AddPolicy(policy.Name, b => b.RequireRole(policy.AllowedRoles));
        }

        public static void AddPolicies(this AuthorizationOptions authOptions)
        {
            authOptions.AddPolicy(PoliciesLookup.Admin);
            authOptions.AddPolicy(PoliciesLookup.Developer);
            authOptions.AddPolicy(PoliciesLookup.Supplier);
            authOptions.AddPolicy(PoliciesLookup.OrganisationOwner);
            authOptions.AddPolicy(PoliciesLookup.Authorized);
        }
    }
}