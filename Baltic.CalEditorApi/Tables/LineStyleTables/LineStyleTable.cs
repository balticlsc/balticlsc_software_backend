using Baltic.CalEditorRegistry.Entities;
using Mighty;

namespace Baltic.CalEditorRegistry.Tables.LineStyleTables
{
    public class LineStyleTable : MightyOrm<LineStyleEntity>
    {
        public LineStyleTable() : base(GlobalConnectionString, "line_style", "Id")
        {

        }
    }
}