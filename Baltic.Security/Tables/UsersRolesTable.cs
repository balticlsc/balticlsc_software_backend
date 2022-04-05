using Mighty;

namespace Baltic.Security.Tables
{
    public class UsersRolesTable : MightyOrm
    {
        public UsersRolesTable() : base(GlobalConnectionString, tableName: "userroles", primaryKeys: "id", sequence: "userroles_id_seq")
        {
            
        }
    }
}