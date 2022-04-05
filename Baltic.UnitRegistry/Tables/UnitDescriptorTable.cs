using Mighty;

namespace Baltic.UnitRegistry.Tables
{
    public class UnitDescriptorTable : MightyOrm
    {
        public UnitDescriptorTable(): base(GlobalConnectionString,tableName:"unitdescriptor",primaryKeys:"id",sequence:"unitdescriptor_id_seq"){}
    }
}