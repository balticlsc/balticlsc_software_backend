using Mighty;

namespace Baltic.UnitRegistry.Tables
{
    public class ReleaseDescriptorTable : MightyOrm
    {
        public ReleaseDescriptorTable(): base(GlobalConnectionString,tableName:"releasedescriptor",primaryKeys:"id",sequence:"releasedescriptor_id_seq"){}
    }
}