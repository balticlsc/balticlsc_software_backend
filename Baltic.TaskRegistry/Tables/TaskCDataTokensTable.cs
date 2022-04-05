using Baltic.TaskRegistry.Entities;
using Mighty;

namespace Baltic.TaskRegistry.Tables
{
    public class TaskCDataTokensTable :MightyOrm<CDataTokenEntity>
    {
        public TaskCDataTokensTable():base(GlobalConnectionString,table: "taskcdatatokens", primaryKeys: "id", sequence: "taskcdatatokens_id_seq")
        {
        }
    }
}
