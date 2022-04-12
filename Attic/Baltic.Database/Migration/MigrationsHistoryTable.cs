using Mighty;

namespace Baltic.Server.Database.Migration
{

    public class MigrationsHistoryTable : MightyOrm<MigrationsHistoryEntity>
    {
        public MigrationsHistoryTable() :
            base(GlobalConnectionString, "MigrationsHistory", "Id")
        { }
    }
}
