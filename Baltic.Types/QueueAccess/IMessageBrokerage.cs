using Baltic.DataModel.CALExecutable;
using Baltic.DataModel.CALMessages;

namespace Baltic.Types.QueueAccess
{
    public interface IMessageBrokerage : IMessageProcessing
    {
        /// 
        /// <param name="qc"></param>
        /// <param name="id"></param>
        short RegisterConsumer(IQueueConsumer qc, QueueId id);

        void ClearTask(string taskUid);
    }
}