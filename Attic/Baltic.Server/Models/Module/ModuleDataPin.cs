#pragma warning disable 1591
namespace Baltic.Server.Models.Module
{
    public enum DataSetType
    {   
        DstFile = 1
    }

    public class ModuleDataPin
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string FullDescription { get; set; }
        public string Direction { get; set; }
        public DataSetType Type { get; set; }
    }
}
