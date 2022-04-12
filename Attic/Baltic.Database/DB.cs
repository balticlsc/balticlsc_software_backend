using System;
using System.Linq;
using System.Reflection;
using Baltic.Server.Database.Migration;
using DbUp;
using Mighty;
using Serilog;

namespace Baltic.Server.Database
{
    public static class DB
    {
        private static string _dbConnectionString;
        public static string ConnectionString
        {
            get => _dbConnectionString;
            set
            {
                _dbConnectionString = value;
                MightyOrm.GlobalConnectionString = $"ProviderName=Npgsql;{value}";
            }
        }

        public static string Version => Current.Query(Scripts.SelectServerVersion).First().version;

        public static string SchemaVersion => $"1.{Current.Query(Scripts.SelectMaxIdFromMigrationsHistory).First().id}";

        public static MightyOrm Current
        {
            get
            {
                if (ConnectionString != null)
                {
                    return new MightyOrm();
                }
                throw new InvalidOperationException("Need a connection string name - can't determine what it is");
            }
        }

        public static int AddMigrations(Type typeInAssembly)
        {
            var db = DeployChanges.To
                        .PostgresqlDatabase(_dbConnectionString)
                        .WithScriptsEmbeddedInAssembly(typeInAssembly.GetTypeInfo().Assembly)
                        .LogToAutodetectedLog()
                        .JournalTo(new Journal());

            db.Configure(x =>
            {
                x.ScriptExecutor.ExecutionTimeoutSeconds = 60 * 5;
                Log.Debug("Configure ExecutionTimeoutSeconds to {ExecutionTimeoutSeconds}", x.ScriptExecutor.ExecutionTimeoutSeconds);
            });

            var result = db.Build().PerformUpgrade();

            if (!result.Successful)
            {
                Log.Error("Migration error", @result.Error);
                return -1;
            }
            return 0;
        }
    }
}
