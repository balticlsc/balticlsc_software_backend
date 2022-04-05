namespace Baltic.CalEditorRegistry.DTO
{
    public class LineEndStyleDTO
    {
        public string Shape { get; set; }
        public string Stroke { get; set; }
        public int StrokeWidth { get; set; }

        public int Radius { get; set; }
        public int Height { get; set; }
        public int Width { get; set; }
        public string Fill { get; set; }
        public bool PerfectDrawEnabled { get; set; }
        public bool Listening { get; set; }
    }
}