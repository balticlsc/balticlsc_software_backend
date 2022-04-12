using System;
using System.Collections.Generic;

namespace Baltic.Server.Controllers.Models
{
    /// <summary>
    /// 
    /// </summary>
    [Obsolete]
    public class ComputationJob
    {
        /// <summary>
        /// 
        /// </summary>
       
        public string Id { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string ComputationTaskId { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string Status { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public DateTime StartTime { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public DateTime EndTime { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int EstimatedCredits { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int UsedCredits { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string ClusterId { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public IEnumerable<ResourceUsageMetric> ResourceUsageMetrics { get; set; }
    }
}
