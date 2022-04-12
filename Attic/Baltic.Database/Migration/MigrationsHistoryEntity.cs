using System;

namespace Baltic.Server.Database.Migration
{
    public class MigrationsHistoryEntity
    {
        public int Id { get; set; }
        public string ScriptName { get; set; }
        public DateTime Applied { get; set; }
    }
}
