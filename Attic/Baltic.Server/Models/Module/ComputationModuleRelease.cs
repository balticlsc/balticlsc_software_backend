#pragma warning disable 1591
using System;
using System.Collections.Generic;
using Baltic.Server.Models.Resources;

namespace Baltic.Server.Models.Module
{
    public class ComputationModuleRelease
    {
        public int Id { get; set; }
        public int ComputationModuleId { get; set; }
        public int AssetId { get; set; }
        public int AuthorId { get; set; }
        public string AuthorName { get; set; }
        public string Version { get; set; }
        public string Description { get; set; }
        public int ParentId { get; set; } // parent
        public IEnumerable<ModuleDataPin> DataSourceDefinition { get; set; }
        public bool Available { get; set; }
        public IEnumerable<ResourceConstraints> InternalResourcesConstraints { get; set; }
        public IEnumerable<ModuleResult> Results { get; set; }
        public DateTime RelaseDate { get; set; }
        public int UsageCounter { get; set; }
        public bool OpenSource { get; set; }
        public bool Private { get; set; }
    }
}
