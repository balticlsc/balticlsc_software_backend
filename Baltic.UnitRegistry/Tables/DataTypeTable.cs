using Mighty;

namespace Baltic.UnitRegistry.Tables
{
    public class DataTypeTable : MightyOrm
    {
        public DataTypeTable(): base(GlobalConnectionString,tableName:"datatype",primaryKeys:"id",sequence:"datatype_id_seq"){}
    }
}