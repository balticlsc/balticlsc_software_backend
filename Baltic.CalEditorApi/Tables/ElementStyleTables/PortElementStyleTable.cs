using Baltic.CalEditorRegistry.Entities;
using Mighty;

namespace Baltic.CalEditorRegistry.Tables.ElementStyleTables
{
    public class PortElementStyleTable : MightyOrm<ElementStyleEntity>
    {
        public PortElementStyleTable() : base(GlobalConnectionString, "port_element_style", "Id")
        {

        }
    }
}