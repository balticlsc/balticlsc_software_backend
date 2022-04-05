using Baltic.CalEditorRegistry.Entities;
using Mighty;

namespace Baltic.CalEditorRegistry.Tables.LocationTables
{
    public class BoxLocationTable : MightyOrm<LocationEntity>
    {
        public BoxLocationTable() : base(GlobalConnectionString, "box_location", "Id")
        {

        }
    }
}