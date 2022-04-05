using Baltic.TaskRegistry.Entities;
using Mighty;

namespace Baltic.TaskRegistry.Tables
{
    public class JobBatchCDataTokensTable : MightyOrm<CDataTokenEntity>
    {
        public JobBatchCDataTokensTable() : base(GlobalConnectionString, table: "jobbatchcdatatokens", primaryKeys: "id", sequence: "jobbatchcdatatokens_id_seq")
        {
        }
    }
}
