using System;
using Baltic.DataModel.CALExecutable;

namespace Baltic.DataModel.CALMessages
{
    public abstract class Message : QueueIdentifiable
    {
        public string MsgUid { get; set; }
        
        public Message()
        {
            MsgUid = $"b-{Guid.NewGuid().ToString()}";
        }
    }
}