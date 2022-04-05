using Baltic.CalEditorRegistry.Model;

namespace Baltic.CalEditorRegistry.DTO
{
    public class LineDTO : ElementDTO
    {
        public string StartElement { get; set; }
        public string EndElement { get; set; }
        public int[] Points { get; set; }
        public LineStyle Style { get; set; } = new LineStyle();
    }
}