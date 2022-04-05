using Baltic.UnitRegistry.Entities;
using Mighty;


namespace Baltic.UnitRegistry.Tables
{
    public class ComputationApplicationReleasesTable : MightyOrm<ComputationApplicationReleaseEntity>
    {
        public ComputationApplicationReleasesTable() : base(GlobalConnectionString, table: "ComputationApplicationReleases", primaryKeys: "Id", sequence: "computationapplicationreleases_id_seq")
        {
        }
    }
}