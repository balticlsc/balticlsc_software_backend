using System.Collections.Concurrent;
using System.Collections.Generic;
using Baltic.DataModel.CALExecutable;
using Baltic.DataModel.CALMessages;
using Baltic.Node.Engine.DataAccess;

namespace Baltic.Node.Engine.DataModel {
	public class JobInstanceInfo
	{
		public JobInstanceMessage JobInstanceMessage { get; set; }
		public IJob Handle { get; set; }
		public bool AcceptsJobs { get; set; }
		
		public long TokensReceived { get; set; }
		public long TokensProcessed { get; set; }

		/// <summary>
		/// string - PinUid, long - counter
		/// </summary>
		public IDictionary<string,long> ProducedTokenCounters { get; set; }
		
		public JobInstanceInfo()
		{
			ProducedTokenCounters = new ConcurrentDictionary<string, long>();
		}

	}
}