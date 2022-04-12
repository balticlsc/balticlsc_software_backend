#pragma warning disable 1591
using System;

namespace Baltic.Server.Models.App
{
    public class ComputationApplicationRelease
    {
        public int Id { get; set; }
        public int ComputationApplicationId { get; set; }
        public string Version { get; set; }
        public string FullDescription { get; set; }
        public bool Available { get; set; }
        public int AuthorId { get; set; }
        public int ParentId { get; set; }
        public string AuthorName { get; set; }
        public int AssetId { get; set; }
        public DateTime ReleaseDate { get; set; }
        public int UsageCounter { get; set; }
        public bool OpenSource { get; set; }
        public bool Private { get; set; }
    }
}
