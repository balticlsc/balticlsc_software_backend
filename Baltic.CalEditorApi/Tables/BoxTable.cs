using Baltic.CalEditorRegistry.Entities;
using Mighty;

namespace Baltic.CalEditorRegistry.Tables
{
    public class BoxTable : MightyOrm<BoxEntity>
    {
        public BoxTable() : base(GlobalConnectionString, "boxes", "_Id")
        {

        }
    }
}