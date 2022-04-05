namespace Baltic.CalEditorRegistry.Model
{
    public class LineStyle
    {
        public string LineType { get; set; }
        public ElementStyle ElementStyle { get; set; }
        public LineEndStyle EndShapeStyle { get; set; }
        public LineEndStyle StartShapeStyle { get; set; }
    }
}