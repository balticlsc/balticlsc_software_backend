using Mighty;

namespace Baltic.UnitRegistry.Tables
{
    public class ComputationApplicationReleaseTable : MightyOrm
    {
        public ComputationApplicationReleaseTable(): base(GlobalConnectionString,tableName:"computationapplicationrelease",primaryKeys:"id",sequence:"computationapplicationrelease_id_seq"){}
    }
}