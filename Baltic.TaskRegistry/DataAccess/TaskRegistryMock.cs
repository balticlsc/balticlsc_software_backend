using System.Collections.Concurrent;
using System.Collections.Generic;
using Baltic.DataModel.CALExecutable;
using Baltic.DataModel.CALMessages;

namespace Baltic.TaskRegistry.DataAccess
{
    public class TaskRegistryMock
    {
        public List<CTask> StoredTasks { get; set; }
        
        /// <summary>
        /// string - queue name, string - job instance Uid
        /// </summary>
        public IDictionary<QueueId, List<string>> EmptiedQueuesToJobs { get; set; }

        public TaskRegistryMock()
        {
            StoredTasks = new List<CTask>();
            EmptiedQueuesToJobs = new ConcurrentDictionary<QueueId, List<string>>();
        }
    }
}