using Mighty;

namespace Baltic.UnitRegistry.Tables
{
    public class DeclaredDataPinTable : MightyOrm
    {
        public DeclaredDataPinTable(): base(GlobalConnectionString,tableName:"declareddatapin",primaryKeys:"id",sequence:"declareddatapin_id_seq"){}
    }
}