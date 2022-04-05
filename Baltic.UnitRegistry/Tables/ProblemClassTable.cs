using Mighty;

namespace Baltic.UnitRegistry.Tables
{
    public class ProblemClassTable : MightyOrm
    {
        public ProblemClassTable(): base(GlobalConnectionString,tableName:"problemclass",primaryKeys:"id",sequence:"problemclass_id_seq"){}
    }
}