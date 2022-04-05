namespace Baltic.CalEditorRegistry.Model
{
    public class Port : Element
    {
        public string ParentId { get; set; }
        public Location Location { get; set; } = new Location();
        public PortStyle Style { get; set; } = new PortStyle();
        public Port()
        {
            Type = DiagramElementType.Port;
        }
    }
}