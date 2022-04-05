using Mighty;

namespace Baltic.UnitRegistry.Tables
{
    public class ComputationModuleTable : MightyOrm
    {
        public ComputationModuleTable(): base(GlobalConnectionString,tableName:"computationmodule",primaryKeys:"id",sequence:"computationmodule_id_seq"){}
    }
}