using Mighty;

namespace Baltic.Security.Tables
{
    public class SessionParamsTable : MightyOrm
    {
        public SessionParamsTable() : base(GlobalConnectionString, tableName: "sessionparams", primaryKeys: "id",
            sequence: "sessionparams_id_seq")
        {
        }
    }
}