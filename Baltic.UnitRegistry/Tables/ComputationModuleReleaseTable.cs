using Mighty;

namespace Baltic.UnitRegistry.Tables
{
    public class ComputationModuleReleaseTable : MightyOrm
    {
        public ComputationModuleReleaseTable(): base(GlobalConnectionString,tableName:"computationmodulerelease",primaryKeys:"id",sequence:"computationmodulerelease_id_seq"){}
    }
}