using Mighty;
using UnitManager.Entity.ComputationAccounts;

namespace Baltic.UnitRegistry.Tables.ComputationAccounts
{
    public class AppShelfTable : MightyOrm<AppShelfEntity>
    {
        public AppShelfTable() : base(GlobalConnectionString, table: "AppShelf", primaryKeys: "Id", sequence: " appshelf_id_seq")
        {
        }
    }
    public class ToolBoxTable : MightyOrm<ToolBoxEntity>
    {
        public ToolBoxTable() : base(GlobalConnectionString, table: " ToolBox", primaryKeys: "Id", sequence: " toolbox_id_seq")
        {
        }
    }
}