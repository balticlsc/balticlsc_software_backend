using Baltic.CalEditorRegistry.Entities;
using Mighty;

namespace Baltic.CalEditorRegistry.Tables.CompartmentTables
{
    public class BoxCompartmentTable : MightyOrm<CompartmentEntity>
    {
        public BoxCompartmentTable() : base(GlobalConnectionString, "box_compartments", "_Id")
        {

        }
    }
}