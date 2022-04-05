using Mighty;

namespace Baltic.Security.Tables
{
    public class UsersInOrganisationTable : MightyOrm
    {
        public UsersInOrganisationTable() : base(GlobalConnectionString, tableName: "usersincompany", primaryKeys: "id",
            sequence: "usersincompany_id_seq")
        {
            
        }
    }
}