using Mighty;
using UnitManager.Entity.ComputationAccounts;

namespace Baltic.UnitRegistry.Tables.ComputationAccounts
{
    public class ApplicationRatingsTable : MightyOrm<UnitRatingEntity>
    {
        public ApplicationRatingsTable() : base(GlobalConnectionString, table: "ApplicationRatings", primaryKeys: "Id", sequence: "unitratings_id_seq")
        {
        }
    }
    public class ModuleRatingsTable : MightyOrm<UnitRatingEntity>
    {
        public ModuleRatingsTable() : base(GlobalConnectionString, table: "ModuleRatings", primaryKeys: "Id", sequence: "unitratings_id_seq")
        {
        }
    }
}