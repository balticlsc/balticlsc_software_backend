namespace Baltic.CalEditorRegistry.Model
{
    public class Compartment
    {
        public string Id { get; set; }
        public string Input { get; set; }
        public string Value { get; set; }
        public string ElementId { get; set; }
        public string CompartmentTypeId { get; set; }
        public CompartmentStyle Style { get; set; } = new CompartmentStyle();
    }
}