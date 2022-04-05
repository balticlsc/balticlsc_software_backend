using Mighty;

namespace Baltic.UnitRegistry.Tables
{
    public class DataStructureTable : MightyOrm
    {
        public DataStructureTable(): base(GlobalConnectionString,tableName:"datastructure",primaryKeys:"id",sequence:"datastructure_id_seq"){}
    }
}