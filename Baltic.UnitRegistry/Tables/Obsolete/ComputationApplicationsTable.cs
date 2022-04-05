using Baltic.UnitRegistry.Entities;
using Mighty;

namespace Baltic.UnitRegistry.Tables
{
    public class ComputationApplicationsTable : MightyOrm<ComputationApplicationEntity>
    {

        public ComputationApplicationsTable() : base(GlobalConnectionString, table: "ComputationApplications", primaryKeys: "Id", sequence: "computationapplications_id_seq")
        {
        }
    }
}