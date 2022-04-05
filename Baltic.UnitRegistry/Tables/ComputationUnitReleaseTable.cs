using Mighty;

namespace Baltic.UnitRegistry.Tables
{
    public class ComputationUnitReleaseTable : MightyOrm
    {
        public ComputationUnitReleaseTable(): base(GlobalConnectionString,tableName:"computationunitrelease",primaryKeys:"id",sequence:"computationunitrelease_id_seq"){}
    }
}