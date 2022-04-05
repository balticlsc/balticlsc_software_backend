using Baltic.TaskRegistry.Entities;
using Mighty;

namespace Baltic.TaskRegistry.Tables
{
    public class CJobsTable : MightyOrm<CJobEntity>
    {
        public CJobsTable() : base(GlobalConnectionString, table: "cjobs", primaryKeys: "id", sequence: "cjobs_id_seq")
        {
        }
    }
}
