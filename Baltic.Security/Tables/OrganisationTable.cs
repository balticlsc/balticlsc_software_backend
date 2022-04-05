using Mighty;

namespace Baltic.Security.Tables
{
    public class OrganisationTable : MightyOrm
    {
        public OrganisationTable() : base(GlobalConnectionString, tableName: "company", primaryKeys: "id", sequence: "company_id_seq")
        {
            
        }
    }
}