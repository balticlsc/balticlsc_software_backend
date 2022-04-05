using Baltic.CalEditorRegistry.Model;

namespace Baltic.CalEditorRegistry.DTO
{
    public class PortDTO : ElementDTO
    {
        public string ParentId { get; set; }
        public Location Location { get; set; } = new Location();
        public PortStyle Style { get; set; } = new PortStyle();
    }
}