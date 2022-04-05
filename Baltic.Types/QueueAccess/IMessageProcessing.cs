using System.Collections.Generic;
using Baltic.DataModel.CALExecutable;
using Baltic.DataModel.CALMessages;

namespace Baltic.Types.QueueAccess
{
    public interface IMessageProcessing 
    {
        /// 
        /// <param name="msg"></param>
        short Enqueue(Message msg);

        /// 
        /// <param name="msgUid"></param>
        /// <param name="queue"></param>
        int Acknowledge(string msgUid, QueueId queue);

        /// 
        /// <param name="queue"></param>
        int CheckQueue(QueueId queue);

        /// 
        /// <param name="id"></param>
        /// <param name="removeMethod"></param>
        short AddQueueFamily(QueueId id, QueueRemoveMethod removeMethod);

        /// 
		/// <param name="family"></param>
		/// <param name="predecessors"></`param>
		short RegisterPredecessors(QueueId family, List<QueueId> predecessors);
    }
}