using Mighty;

namespace Baltic.UnitRegistry.Tables
{
    public class ComputationUnitTable : MightyOrm
    {
        public ComputationUnitTable(): base(GlobalConnectionString,tableName:"computationunit",primaryKeys:"id",sequence:"computationunit_id_seq"){}
    }
}