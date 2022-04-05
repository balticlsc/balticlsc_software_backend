using Baltic.UnitRegistry.Entities;
using Mighty;


namespace Baltic.UnitRegistry.Tables

{
    public class ComputationModulesTable : MightyOrm<ComputationModuleEntity>
    {
        public ComputationModulesTable() : base(GlobalConnectionString, table: "ComputationModules", primaryKeys: "Id", sequence: "compuationmodules_id_seq")
        {
        }
    }
}