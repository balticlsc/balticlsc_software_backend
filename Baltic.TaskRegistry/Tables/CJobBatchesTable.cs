using Baltic.TaskRegistry.Entities;
using Mighty;

namespace Baltic.TaskRegistry.Tables
{
    public class CJobBatchesTable : MightyOrm<CJobBatchEntity>
    {
        public CJobBatchesTable() : base(GlobalConnectionString, table: "cjobbatches", primaryKeys: "id", sequence: "cjobbatches_id_seq")
        {
        }
    }
}
