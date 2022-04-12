using Baltic.Server.Models.Resources;

namespace Baltic.Server.Models.Jobs
{
    public class JobHeader //from JobStart in UML
    {
        public string AssetId { get; set; }
        public string JobId { get; set; }
        public string DataSourceDefinition { get; set; }
        public ResourceConstraints ResourceConstraints { get; set; } //not sure about IEnumerable<ResourceConstraints>
        public string Context { get; set; } //JSON params list
                                            // lista parametrów musi zostać zrobiona przez coś po stronie aplikacji
    }
}
