#pragma warning disable 1591
using System;
using System.Collections.Generic;

namespace Baltic.Server.Models.App
{
    public class ComputationApplication
    {
        public int Id { get; set; }
        public int ForkId { get; set; } //parent
        public string Name { get; set; }
        public string ShortDescription { get; set; }
        public string FullDescription { get; set; }
        public int Rate { get; set; } //1-5
        public int AuthorId { get; set; }
        public string AuthorFullName { get; set; }
        public string Icon { get; set; }
        public IEnumerable<string> Keywords { get; set; }
        public DateTime LastUpdateDate { get; set; }
        public int TimesUsed { get; set; } // minuty
        public bool OpenSource { get; set; }
        public IEnumerable<ComputationApplicationRelease> Releases { get; set; }
    }
}
