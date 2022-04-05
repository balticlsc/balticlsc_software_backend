using Baltic.CalEditorRegistry.Entities;
using Mighty;

namespace Baltic.CalEditorRegistry.Tables.CompartmentStyleTables
{
    public class LineCompartmentStyleTable : MightyOrm<CompartmentStyleEntity>
    {
        public LineCompartmentStyleTable() : base(GlobalConnectionString, "line_compartment_style", "Id")
        {

        }
    }
}