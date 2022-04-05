using System.Collections.Generic;

namespace Baltic.DataModel.Resources
{
    public class CMachine
    {
        public string Uid { get; set; }
        public List<string> CPUs { get; set; }
        public List<string> GPUs { get; set; }
        public List<string> Memory { get; set; }
    }
}