using Baltic.TaskRegistry.Entities;
using Mighty;

namespace Baltic.TaskRegistry.Tables
{
    public class CTasksTable : MightyOrm<CTaskEntity>
    {
        public CTasksTable() : base(GlobalConnectionString, table: "ctasks", primaryKeys: "id", sequence: "ctasks_id_seq")
        {
        }
    }
}
