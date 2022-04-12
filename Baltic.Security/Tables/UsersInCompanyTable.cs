using Mighty;

namespace Baltic.Security.Tables
{
    public class UsersInCompanyTable : MightyOrm
    {
        public UsersInCompanyTable() : base(GlobalConnectionString, tableName: "usersincompany", primaryKeys: "id",
            sequence: "usersincompany_id_seq")
        {
            
        }
    }
}