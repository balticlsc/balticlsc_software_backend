using Mighty;

namespace Baltic.UnitRegistry.Tables
{
    public class ToolboxTable : MightyOrm
    {
        public ToolboxTable(): base(GlobalConnectionString,tableName:"toolbox",primaryKeys:"id",sequence:"toolbox_id_seq"){}
    }
}