using Baltic.CalEditorRegistry.Entities;
using Mighty;

namespace Baltic.CalEditorRegistry.Tables.CompartmentTables
{
    public class PortCompartmentTable : MightyOrm<CompartmentEntity>
    {
        public PortCompartmentTable() : base(GlobalConnectionString, "port_compartments", "_Id")
        {

        }
    }
}