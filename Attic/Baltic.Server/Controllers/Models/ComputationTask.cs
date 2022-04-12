using System;
using System.Collections.Generic;
namespace Baltic.Server.Controllers.Models
{
    /// <summary>
    /// 
    /// </summary>
    [Obsolete]
    public class ComputationTask
    {
        /// <summary>
        /// 
        /// </summary>
        public string Id { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string ComputationApplicationReleaseId { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string UserId { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public IEnumerable<Server.Models.Jobs.ComputationJob> JobList { get; set; } // możliwy paginator
        /// <summary>
        /// 
        /// </summary>
        public string Status { get; set; }
    }
}
