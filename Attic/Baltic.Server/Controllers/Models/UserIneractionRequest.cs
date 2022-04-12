using System;

namespace Baltic.Server.Controllers.Models
{
    /// <summary>
    /// 
    /// </summary>
    [Obsolete]
    public class UserInteractionRequest
    {
        /// <summary>
        /// 
        /// </summary>
        public string Id { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string taskId { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string jobId { get; set; }   // id joba który będzie potrzebował tych danych
        /// <summary>
        /// 
        /// </summary>
        public string contexDefinition { get; set; }
    }
}