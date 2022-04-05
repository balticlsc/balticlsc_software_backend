using Mighty;

namespace Baltic.UnitRegistry.Tables
{
    public class DataTypeToAccessTypeTable : MightyOrm
    {
        public DataTypeToAccessTypeTable(): base(GlobalConnectionString,tableName:"datatypetoaccesstype",primaryKeys:"id",sequence:"datatypetoaccesstype_id_seq"){}
    }
}