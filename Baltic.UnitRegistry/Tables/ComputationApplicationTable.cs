using Mighty;

namespace Baltic.UnitRegistry.Tables
{
    public class ComputationApplicationTable : MightyOrm
    {
        public ComputationApplicationTable(): base(GlobalConnectionString,tableName:"computationapplication",primaryKeys:"id",sequence:"computationapplication_id_seq"){}
    }
}