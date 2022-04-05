using Mighty;

namespace Baltic.Security.Tables
{
    public class UsersExtensionTable : MightyOrm
    {
        public UsersExtensionTable() : base(GlobalConnectionString, tableName: "userextension", primaryKeys: "id",
            sequence: "userextension_id_seq")
        {
        }
    }
}