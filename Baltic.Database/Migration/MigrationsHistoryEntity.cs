using System;

namespace Baltic.Database.Migration
{
    public class MigrationsHistoryEntity
    {
        public int Id { get; set; }
        public string ScriptName { get; set; }
        public DateTime Applied { get; set; }
    }
}
