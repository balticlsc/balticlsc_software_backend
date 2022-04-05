using Baltic.CalEditorRegistry.Entities;
using Mighty;

namespace Baltic.CalEditorRegistry.Tables.CompartmentStyleTables
{
    public class BoxCompartmentStyleTable : MightyOrm<CompartmentStyleEntity>
    {
        public BoxCompartmentStyleTable() : base(GlobalConnectionString, "box_compartment_style", "Id")
        {

        }
    }
}