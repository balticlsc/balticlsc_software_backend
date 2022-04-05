using Mighty;

namespace Baltic.UnitRegistry.Tables
{
    public class CommandArgumentTable : MightyOrm
    {
        public CommandArgumentTable(): base(GlobalConnectionString,tableName:"commandargument",primaryKeys:"id",sequence:"commandargument_id_seq"){}
    }
}