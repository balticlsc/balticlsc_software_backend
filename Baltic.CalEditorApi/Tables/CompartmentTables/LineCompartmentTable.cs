using Baltic.CalEditorRegistry.Entities;
using Mighty;

namespace Baltic.CalEditorRegistry.Tables.CompartmentTables
{
    public class LineCompartmentTable : MightyOrm<CompartmentEntity>
    {
        public LineCompartmentTable() : base(GlobalConnectionString, "line_compartments", "_Id")
        {

        }
    }
}