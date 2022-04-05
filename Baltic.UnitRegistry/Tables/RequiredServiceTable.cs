using Mighty;

namespace Baltic.UnitRegistry.Tables
{
    public class RequiredServiceTable : MightyOrm
    {
        public RequiredServiceTable(): base(GlobalConnectionString,tableName:"requiredservice",primaryKeys:"id",sequence:"requiredservice_id_seq"){}
    }
}