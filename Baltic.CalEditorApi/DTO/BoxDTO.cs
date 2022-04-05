using Baltic.CalEditorRegistry.Model;

namespace Baltic.CalEditorRegistry.DTO
{
    public class BoxDTO : ElementDTO
    {
        public BoxStyle Style { get; set; } = new BoxStyle();
        public Location Location { get; set; }
    }
}