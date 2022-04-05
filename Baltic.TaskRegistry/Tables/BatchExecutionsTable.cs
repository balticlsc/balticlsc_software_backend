using Baltic.TaskRegistry.Entities;
using Mighty;

namespace Baltic.TaskRegistry.Tables
{
    public class BatchExecutionsTable : MightyOrm<BatchExecutionEntity>
    {
        public BatchExecutionsTable() : base(GlobalConnectionString, table: "batchexecutions", primaryKeys: "id", sequence: "batchexecutions_id_seq")
        {
        }
    }
}
