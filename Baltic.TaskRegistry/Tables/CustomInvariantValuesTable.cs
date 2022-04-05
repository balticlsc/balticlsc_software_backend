using Baltic.TaskRegistry.Entities;
using Mighty;

namespace Baltic.TaskRegistry.Tables
{
    public class CustomInvariantValuesTable : MightyOrm<CustomInvariantValueEntity>
    {
        public CustomInvariantValuesTable() : base(GlobalConnectionString, table: "custominvariantvalues", primaryKeys: "id", sequence: "custominvariantvalues_id_seq")
        {
        }
    }
}
