using System;
using System.Data;
using System.Linq;
using DbUp.Engine;

namespace Baltic.Database.Migration
{
    public class Journal : IJournal
    {
        public void EnsureTableExistsAndIsLatestVersion(Func<IDbCommand> dbCommandFactory)
        {
        }

        public string[] GetExecutedScripts()
        {
            // Create migrations history journal if not exists.
            DB.Current.Execute(Scripts.CreateMigrationsHistory);

            // Get scripts.
            var migrationsHistory = new MigrationsHistoryTable();

            return migrationsHistory
                .All()
                .OrderBy(x => x.Applied)
                .Select(x => x.ScriptName)
                .ToArray();
        }

        public void StoreExecutedScript(SqlScript script, Func<IDbCommand> dbCommandFactory)
        {
            var migrationsHistory = new MigrationsHistoryTable();
            migrationsHistory.Save(new { ScriptName = script.Name, Applied = DateTime.Now });
        }
    }
}
