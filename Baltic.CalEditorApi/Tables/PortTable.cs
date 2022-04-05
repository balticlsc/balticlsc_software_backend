using Baltic.CalEditorRegistry.Entities;
using Mighty;

namespace Baltic.CalEditorRegistry.Tables
{
    public class PortTable : MightyOrm<PortEntity>
    {
        public PortTable() : base(GlobalConnectionString, "ports", "_Id")
        {

        }
    }
}