using System;
using Baltic.Types.QueueAccess;

namespace Baltic.Queue.MultiQueue
{
    public class QueueConsumersHandle
    {
        public IQueueConsumer Handle;
        public DateTime LastFailedConnection;
        public int NumberOfConsumerInstances;
        public int Counter;

        public QueueConsumersHandle(IQueueConsumer handle)
        {
            Handle = handle;
            LastFailedConnection = DateTime.MinValue;
            NumberOfConsumerInstances = 1;
            Counter = 0;
        }
    }
}