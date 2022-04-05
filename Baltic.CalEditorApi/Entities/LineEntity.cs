namespace Baltic.CalEditorRegistry.Entities
{
    public class LineEntity : Element
    {
        public string StartElement { get; set; }
        public string EndElement { get; set; }
        public int[] Points { get; set; }
    }
}