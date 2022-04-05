using Baltic.TaskRegistry.Entities;
using Mighty;

namespace Baltic.TaskRegistry.Tables
{
    public class JobExecutionsTable :MightyOrm<JobExecutionEntity>
    {
        public JobExecutionsTable() : base(GlobalConnectionString, table: "jobexecutions", primaryKeys: "id", sequence: "jobexecutions_id_seq")
        { 
        }
    }
}
