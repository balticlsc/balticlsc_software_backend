namespace Baltic.CalEditorRegistry.Model
{
    public class Line : Element
    {
        public string StartElement { get; set; }
        public string EndElement { get; set; }
        public int[] Points { get; set; }
        public LineStyle Style { get; set; }

        public Line()
        {
            Type = DiagramElementType.Line;
        }
    }
}