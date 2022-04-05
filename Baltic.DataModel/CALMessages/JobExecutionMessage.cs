using System.Collections.Concurrent;
using System.Collections.Generic;
using Baltic.DataModel.CALExecutable;

namespace Baltic.DataModel.CALMessages
{
    public class JobExecutionMessage : Message
    {
        public string BatchUid { get; set; }
        public string JobUid { get; set; }
        
        /// <summary>
        /// string - PinUid
        /// </summary>
        public IDictionary<string, QueueId> RequiredPinQueues { get; set; }
        
        public override string FamilyId => BatchUid + "." + JobUid;

        public JobExecutionMessage()
        {
            BatchUid = "";
            JobUid = "";
            RequiredPinQueues = new ConcurrentDictionary<string, QueueId>();
        }
        
        public override string ToString()
        {
            var i = 0;
            var ret = "JobExecutionMessage TaskUid - " + TaskUid + " MsgUid - " + MsgUid + "\n";
            foreach (var q in RequiredPinQueues)
            {
                ret = ret + "\t  Queue " + i + ": " + q.ToString() + "\n";
                i++;
            }

            return ret;
        }
    }
}