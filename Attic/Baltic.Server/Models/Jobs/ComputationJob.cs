using System;
using System.Collections.Generic;
using Baltic.Server.Models.Resources;

namespace Baltic.Server.Models.Jobs
{ 
    public class ComputationJob //from Job & JobStatus in UML
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string ComputationTaskId { get; set; }
        public string ComputationModuleReleaseId { get; set; }
        public string ComputationModuleName { get; set; }
        public string ClusterId { get; set; }
        public float Progress { get; set; }
        public string Status { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public int ReservedCredits { get; set; }
        public int EstimatedCredits { get; set; }
        public int UsedCredits { get; set; }
        public IEnumerable<ResourceUsageMetric> ResourceUsageMetrics { get; set; }
    }
}
