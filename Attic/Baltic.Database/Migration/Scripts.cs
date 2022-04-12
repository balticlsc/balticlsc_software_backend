namespace Baltic.Server.Database.Migration
{
    public class Scripts
    {
        public const string CreateMigrationsHistory = "CREATE TABLE IF NOT EXISTS MigrationsHistory (Id SERIAL PRIMARY KEY NOT NULL, ScriptName VARCHAR(255), Applied TIMESTAMP);";
        public const string SelectServerVersion = "SELECT version();";
        public const string SelectMaxIdFromMigrationsHistory = "SELECT coalesce(max(id), 0) as id FROM public.migrationshistory;";
    }
}
