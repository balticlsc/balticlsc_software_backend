using System;
using Newtonsoft.Json;

namespace Baltic.Server.Controllers.Models
{
    /// <summary>
    /// 
    /// </summary>
    [Obsolete]
    public class ErrorDetails
    {
        /// <summary>
        /// 
        /// </summary>
        public int StatusCode { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string Message { get; set; }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return JsonConvert.SerializeObject(this);
        }
    }
}

