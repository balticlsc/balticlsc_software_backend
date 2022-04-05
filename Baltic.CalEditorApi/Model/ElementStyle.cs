namespace Baltic.CalEditorRegistry.Model
{
    public class ElementStyle
    {
        public string Fill { get; set; }
        public int Opacity { get; set; }
        public string Stroke { get; set; }
        public int StrokeWidth { get; set; }
        public string Shape { get; set; }
        public bool PerfectDrawEnabled { get; set; }
        public int[] Dash { get; set; }
    }
}