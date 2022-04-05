using Mighty;

namespace Baltic.UnitRegistry.Tables
{
    public class KeywordTable : MightyOrm
    {
        public KeywordTable(): base(GlobalConnectionString,tableName:"keyword",primaryKeys:"id",sequence:"keyword_id_seq"){}
    }
}