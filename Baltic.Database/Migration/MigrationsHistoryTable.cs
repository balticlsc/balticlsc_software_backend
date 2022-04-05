using Mighty;

namespace Baltic.Database.Migration
{

    public class MigrationsHistoryTable : MightyOrm<MigrationsHistoryEntity>
    {
        public MigrationsHistoryTable() :
            base(GlobalConnectionString, "MigrationsHistory", "Id")
        { }
    }
}
