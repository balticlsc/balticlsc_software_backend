using System;

namespace Baltic.Server.Controllers.Models
{
    /// <summary>
    /// 
    /// </summary>
    [Obsolete]
    public class ComputationApplicationEnvironment
    {
        /// <summary>
        /// 
        /// </summary>
        public string Context { get; set; } //JSON params list
                                            // lista parametrów musi zostać zrobiona przez coś po stronie aplikacji
        /// <summary>
        /// 
        /// </summary>
        public string ComputationValuation { get; set; } // do wyciągnięcia
        /// <summary>
        /// 
        /// </summary>
        public string DataSourceDefinition{ get; set; }
    }
}
