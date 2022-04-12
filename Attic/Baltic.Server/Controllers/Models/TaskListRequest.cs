using System;

namespace Baltic.Server.Controllers.Models
{
    /// <summary>
    /// 
    /// </summary>
    [Obsolete]
    public class TaskListRequest // co mamy zwrócić   listę zadań globalnych / firmowych / użytkownika / działające lub nie / zakończone / przerwane / w toku / z wtorku /
    {                               // wymagających interakcji
        /// <summary>
        /// 
        /// </summary>
        public string UserId { get; set; }
    }
}
