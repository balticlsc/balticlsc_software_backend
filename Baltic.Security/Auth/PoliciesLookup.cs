namespace Baltic.Security.Auth
{
    public static class PoliciesLookup
    {
        public const string RequireAdmin = "Admin";
        public const string RequireOrganisationAdmin = "Organisation Admin";
        public const string RequireSupplier = "Supplier";
        public const string RequireDeveloper = "Developer";
        public const string RequireDemo = "Demo";
        public const string RequireAuthentication = "Authorized";

        public static Policy Admin { get; } = new Policy
        {
            Name = RequireAdmin,
            AllowedRoles = new[] {RoleLookup.Supervisor}
        };

        public static Policy OrganisationOwner { get; } = new Policy
        {
            Name = RequireOrganisationAdmin,
            AllowedRoles = new[] {RoleLookup.OrganisationAdmin}
        };

        public static Policy Supplier { get; } = new Policy
        {
            Name = RequireSupplier,
            AllowedRoles = new[] {RoleLookup.Supplier}
        };

        public static Policy Developer { get; } = new Policy
        {
            Name = RequireDeveloper,
            AllowedRoles = new[] {RoleLookup.Developer}
        };

        public static Policy Demo { get; } = new Policy
        {
            Name = RequireDemo,
            AllowedRoles = new[] {RoleLookup.Demo}
        };

        public static Policy Authorized { get; } = new Policy
        {
            Name = RequireAuthentication,
            AllowedRoles = RoleLookup.GetAll
        };

        public class Policy
        {
            public string Name { get; set; }
            public string[] AllowedRoles { get; set; }
        }        
    }
}