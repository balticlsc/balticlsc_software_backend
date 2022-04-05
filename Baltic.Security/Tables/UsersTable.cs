using Mighty;

namespace Baltic.Security.Tables
{
    public class UsersTable : MightyOrm
    {
        public UsersTable() : base(GlobalConnectionString, tableName: "users", primaryKeys: "id", 
            sequence: "users_id_seq") 
        {
        }
    }
}