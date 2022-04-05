using Mighty;

namespace Baltic.Security.Tables
{
    public class SessionTable : MightyOrm
    {
        public SessionTable() : base(GlobalConnectionString, tableName: "session", primaryKeys: "id",
            sequence: "session_id_seq")
        {
        }
    }
}