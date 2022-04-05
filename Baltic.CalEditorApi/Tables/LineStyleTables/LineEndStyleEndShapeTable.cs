using Baltic.CalEditorRegistry.Entities;
using Mighty;

namespace Baltic.CalEditorRegistry.Tables.LineStyleTables
{
    public class LineEndStyleEndShapeTable : MightyOrm<LineEndStyleEntity>
    {
        public LineEndStyleEndShapeTable() : base(GlobalConnectionString, "line_end_style_end_shape", "Id")
        {

        }
    }
}