using Mighty;

namespace Baltic.UnitRegistry.Tables
{
    public class UnitParameterTable : MightyOrm
    {
        public UnitParameterTable(): base(GlobalConnectionString,tableName:"unitparameter",primaryKeys:"id",sequence:"unitparameter_id_seq"){}
    }
}