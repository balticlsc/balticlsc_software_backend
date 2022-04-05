using Baltic.CalEditorRegistry.Entities;
using Mighty;

namespace Baltic.CalEditorRegistry.Tables.LocationTables
{
    public class PortLocationTable : MightyOrm<LocationEntity>
    {
        public PortLocationTable() : base(GlobalConnectionString, "port_location", "Id")
        {

        }
    }
}