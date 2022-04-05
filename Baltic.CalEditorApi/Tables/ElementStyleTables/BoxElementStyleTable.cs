using Baltic.CalEditorRegistry.Entities;
using Mighty;

namespace Baltic.CalEditorRegistry.Tables.ElementStyleTables
{
    public class BoxElementStyleTable : MightyOrm<ElementStyleEntity>
    {
        public BoxElementStyleTable() : base(GlobalConnectionString, "box_element_style", "Id")
        {

        }
    }
}