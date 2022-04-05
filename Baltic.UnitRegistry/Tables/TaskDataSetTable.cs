using Mighty;

namespace Baltic.UnitRegistry.Tables
{
    public class TaskDataSetTable : MightyOrm
    {
        public TaskDataSetTable(): base(GlobalConnectionString,tableName:"taskdataset",primaryKeys:"id",sequence:"taskdataset_id_seq"){}
    }
}