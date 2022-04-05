using Mighty;
using UnitManager.Entity;

namespace Baltic.UnitRegistry.Tables.Table.ComputationAccounts
{
    public class ApplicationDescriptorsTable : MightyOrm<UnitDescriptorEntity>
    {
        public ApplicationDescriptorsTable() : base(GlobalConnectionString, table: "ApplicationDescriptors", primaryKeys: "Id", sequence: " applicationdescriptors_id_seq")
        {
        }
    }
    public class ModuleDescriptorsTable : MightyOrm<UnitDescriptorEntity>
    {
        public ModuleDescriptorsTable() : base(GlobalConnectionString, table: "ModuleDescriptors", primaryKeys: "Id", sequence: " moduledescriptors_id_seq")
        {
        }
    }
}