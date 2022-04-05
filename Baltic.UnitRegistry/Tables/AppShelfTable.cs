using Mighty;

namespace Baltic.UnitRegistry.Tables
{
    public class AppShelfTable : MightyOrm
    {
        public AppShelfTable(): base(GlobalConnectionString,tableName:"appshelf",primaryKeys:"id",sequence:"appshelf_id_seq"){}
    }
}