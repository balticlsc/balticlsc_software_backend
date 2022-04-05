namespace Baltic.Security.Auth
{
    public static class RoleLookup
    {
        public const string Supervisor = "Admin";
        public const string OrganisationAdmin = "Organisation Admin";
        public const string Supplier = "Supplier";
        public const string Developer = "Developer";
        public const string Demo = "Demo";        
        public const string Basic = "Basic User";

        public static string[] GetAll => new []
        {
            Supervisor,
            OrganisationAdmin,
            Supplier,
            Developer,
            Demo,
            Basic
        };        
    }
}