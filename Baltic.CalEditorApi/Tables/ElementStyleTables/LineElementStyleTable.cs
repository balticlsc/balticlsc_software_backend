using Baltic.CalEditorRegistry.Entities;
using Mighty;

namespace Baltic.CalEditorRegistry.Tables.ElementStyleTables
{
    public class LineElementStyleTable : MightyOrm<ElementStyleEntity>
    {
        public LineElementStyleTable() : base(GlobalConnectionString, "line_element_style", "Id")
        {

        }
    }
}