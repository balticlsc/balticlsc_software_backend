using Baltic.CalEditorRegistry.Model;

namespace Baltic.CalEditorRegistry.DTO
{
    public class LineStyleDTO
    {
        public string LineType { get; set; }
        public ElementStyle ElementStyle { get; set; }
        public LineEndStyle EndShapeStyle { get; set; }
        public LineEndStyle StartShapeStyle { get; set; }
    }
}