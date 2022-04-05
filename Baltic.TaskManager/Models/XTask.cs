using System;
using System.Collections.Generic;
using Baltic.DataModel.Types;

namespace Baltic.TaskManager.Models
{
	public class XTask
	{
		// from: CTask
		public string Uid { get; set; }
		public string ReleaseUid { get; set; }
		public IEnumerable<XBatch> Batches { get; set; }
		
		// from: ExecutionRecord
		public ComputationStatus Status { get; set; }
		public DateTime Start { get; set; }
		public DateTime Finish { get; set; }
		
		// from: TaskExecution
		public float ConsumedCredits { get; set; }
		public bool IsArchived { get; set; }
		public XTaskParameters Parameters { get; set; }
		public long TokensReceived { get; set; }
		public long TokensProcessed { get; set; }
	}
}