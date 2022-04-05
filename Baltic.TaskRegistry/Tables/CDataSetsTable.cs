using Baltic.TaskRegistry.Entities;
using Mighty;

namespace Baltic.TaskRegistry.Tables
{
    public class CDataSetsTable :MightyOrm<CDataSetEntity>
    {
        public CDataSetsTable() :base(GlobalConnectionString, table: "cdatasets", primaryKeys: "id", sequence: "cdatasets_id_seq")
        {
        }
    }

}