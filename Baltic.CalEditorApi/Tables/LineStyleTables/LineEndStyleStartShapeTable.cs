using Baltic.CalEditorRegistry.Entities;
using Mighty;

namespace Baltic.CalEditorRegistry.Tables.LineStyleTables
{
    public class LineEndStyleStartShapeTable : MightyOrm<LineEndStyleEntity>
    {
        public LineEndStyleStartShapeTable() : base(GlobalConnectionString, "line_end_style_start_shape", "Id")
        {

        }
    }
}