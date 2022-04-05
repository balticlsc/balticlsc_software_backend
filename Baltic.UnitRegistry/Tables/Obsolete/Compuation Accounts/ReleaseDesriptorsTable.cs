using Mighty;
using UnitManager.Entity.ComputationAccounts;

namespace Baltic.UnitRegistry.Tables.ComputationAccounts
{
    public class ReleaseDescriptorsForComputationModuleReleaseTable : MightyOrm<ReleaseDescriptorEntity>
    {
        public ReleaseDescriptorsForComputationModuleReleaseTable() : base(GlobalConnectionString, table: "ReleaseDescriptorsForComputationModuleRelease", primaryKeys: "Id", sequence: "releasedescriptors_id_seq")
        {
        }
    }
    public class ReleaseDescriptorsForComputationApplicationReleaseTable : MightyOrm<ReleaseDescriptorEntity>
    {
        public ReleaseDescriptorsForComputationApplicationReleaseTable() : base(GlobalConnectionString, table: "ReleaseDescriptorsForComputationApplicationRelease", primaryKeys: "Id", sequence: "releasedescriptors_id_seq")
        {
        }
    }
}