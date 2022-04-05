using System.Collections.Generic;
using Baltic.DataModel.CALExecutable;

namespace Baltic.DataModel.CALMessages {
	public class BatchExecutionMessage : Message {
		public string BatchUid { get; set; }
		public List<QueueId> JobQueueIds { get; set; }
		
		public override string FamilyId => BatchUid;
		
		public BatchExecutionMessage()
		{
			BatchUid = "";
			JobQueueIds = new List<QueueId>();
		}
	}
}