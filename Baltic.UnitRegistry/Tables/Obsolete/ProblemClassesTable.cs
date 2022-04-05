using Baltic.UnitRegistry.AppStore;
using Mighty;


namespace Baltic.UnitRegistry.Tables.Table
{
    public class ProblemClassesTable : MightyOrm<ProblemClassEntity>
    {
        public ProblemClassesTable() : base(GlobalConnectionString, table: "ProblemClasses", primaryKeys: "Id", sequence: "problemclasses_id_seq")
        {
        }
    }
}