using Baltic.CalEditorRegistry.Entities;
using Mighty;

namespace Baltic.CalEditorRegistry.Tables
{
    public class LineTable : MightyOrm<LineEntity>
    {
        public LineTable() : base(GlobalConnectionString, "lines", "_Id")
        {
            
        }
    }
}