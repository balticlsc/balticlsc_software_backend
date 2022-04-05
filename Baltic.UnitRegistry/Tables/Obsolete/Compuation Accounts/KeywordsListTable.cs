using Mighty;
using UnitManager.Entity.ComputationAccounts;

namespace Baltic.UnitRegistry.Tables.ComputationAccounts
{
    public class KeywordsListForComputationApplicationTable : MightyOrm<ReleaseDescriptorEntity>
    {
        public KeywordsListForComputationApplicationTable() : base(GlobalConnectionString, table: "KeywordsListForComputationApplication")
        {
        }
    }

    public class KeywordsListForComputationModuleTable : MightyOrm<ReleaseDescriptorEntity>
    {
        public KeywordsListForComputationModuleTable() : base(GlobalConnectionString, table: "KeywordsListForComputationModule")
        {
        }
    }
}