using Mighty;

namespace Baltic.Security.Tables
{
    public class CompaniesTable : MightyOrm
    {
        public CompaniesTable() : base(GlobalConnectionString, tableName: "company", primaryKeys: "id", sequence: "company_id_seq")
        {
            
        }
    }
}