namespace Baltic.CalEditorRegistry.Model
{
    public class Box : Element
    {
        public Location Location { get; set; } = new Location();
        public BoxStyle Style { get; set; } = new BoxStyle();

        public Box()
        {
            Type = DiagramElementType.Box;
        }
    }
}