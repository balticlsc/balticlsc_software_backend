using Baltic.CalEditorRegistry.Entities;
using Mighty;

namespace Baltic.CalEditorRegistry.Tables
{
    public class DiagramTable : MightyOrm<DiagramEntity>
    {
        public DiagramTable() : base(GlobalConnectionString, "diagrams", primaryKeys:"_Id")
        {

        }
    }
}