using Mighty;

namespace Baltic.UnitRegistry.Tables
{
    public class AccessTypeTable : MightyOrm
    {
        public AccessTypeTable(): base(GlobalConnectionString,tableName:"accesstype",primaryKeys:"id",sequence:"accesstype_id_seq"){}
    }
}