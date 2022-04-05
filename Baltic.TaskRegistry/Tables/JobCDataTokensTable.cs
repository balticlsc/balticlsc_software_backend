using Baltic.TaskRegistry.Entities;
using Mighty;

namespace Baltic.TaskRegistry.Tables
{
    public class JobCDataTokensTable : MightyOrm<CDataTokenEntity>
    {
        public JobCDataTokensTable() : base(GlobalConnectionString, table: "jobcdatatokens", primaryKeys: "id", sequence: "jobcdatatokens_id_seq")
        {
        }
    }
}
