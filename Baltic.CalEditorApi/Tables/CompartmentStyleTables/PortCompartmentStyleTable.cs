using Baltic.CalEditorRegistry.Entities;
using Mighty;

namespace Baltic.CalEditorRegistry.Tables.CompartmentStyleTables
{
    public class PortCompartmentStyleTable : MightyOrm<CompartmentStyleEntity>
    {
        public PortCompartmentStyleTable() : base(GlobalConnectionString, "port_compartment_style", "Id")
        {

        }
    }
}