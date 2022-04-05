using Baltic.TaskRegistry.Entities;
using Mighty;

namespace Baltic.TaskRegistry.Tables
{
    public class ResourceUsagesTable : MightyOrm<ResourceUsageEntity>
    {
        public ResourceUsagesTable() :base(GlobalConnectionString, table: "resourceusages", primaryKeys:"id",sequence:"resourceusages_id_seq")
        {
        }
    }
}