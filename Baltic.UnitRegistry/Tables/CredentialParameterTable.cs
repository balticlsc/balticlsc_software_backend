using Mighty;

namespace Baltic.UnitRegistry.Tables
{
    public class CredentialParameterTable : MightyOrm
    {
        public CredentialParameterTable(): base(GlobalConnectionString,tableName:"credentialparameter",primaryKeys:"id",sequence:"credentialparameter_id_seq"){}
    }
}