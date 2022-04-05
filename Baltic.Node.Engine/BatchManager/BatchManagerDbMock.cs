using System.Collections.Concurrent;
using System.Collections.Generic;
using Baltic.Node.Engine.DataModel;

namespace Baltic.Node.Engine.BatchManager
{
    public class BatchManagerDbMock
    {
        /// <summary>
        /// string - jobs_queue_uid
        /// </summary>
        public IDictionary<string,BatchInstanceInfo> BatchInfos;

        public BatchManagerDbMock()
        {
            BatchInfos = new ConcurrentDictionary<string, BatchInstanceInfo>();
        }
    }
}