using Baltic.TaskRegistry.Entities;
using Mighty;

namespace Baltic.TaskRegistry.Tables
{
    public class TaskExecutionsTable : MightyOrm<TaskExecutionEntity>
    {
        public TaskExecutionsTable() : base(GlobalConnectionString, table: "taskexecutions", primaryKeys: "id", sequence: "taskexecutions_id_seq")
        {
        }
    }
}
