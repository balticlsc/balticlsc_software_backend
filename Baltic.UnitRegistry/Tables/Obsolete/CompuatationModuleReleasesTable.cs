using Baltic.UnitRegistry.Entities;
using Mighty;

namespace Baltic.UnitRegistry.Tables
{
    public class ComputationModuleReleasesTable : MightyOrm<ComputationModuleReleaseEntity>
    {
        public ComputationModuleReleasesTable() : base(GlobalConnectionString, table: "ComputationModuleReleases", primaryKeys: "Id", sequence: "computationmodulereleases_id_seq")
        {
        }
    }
}