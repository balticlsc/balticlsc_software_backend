using Mighty;

namespace Baltic.Security.Tables
{
    public class UserPreferencesTable : MightyOrm
    {
        public UserPreferencesTable() : base(GlobalConnectionString, tableName: "userpreferences", primaryKeys: "id",
            sequence: "userpreferences_id_seq")
        {
        }
    }
}