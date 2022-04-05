using System.Collections.Concurrent;
using System.Collections.Generic;
using Baltic.DataModel.CALExecutable;
using Baltic.DataModel.CALMessages;

namespace Baltic.Node.Engine.DataModel {
	public class BatchInstanceInfo {
		public BatchInstanceMessage BatchInstanceMessage { get; set; }
		/// <summary>
		/// string - PhysicalJobInstanceUid
		/// </summary>
		public IDictionary<string,JobInstanceInfo> JobInfos { get; set; }
		/// <summary>
		/// string - msg_uid
		/// </summary>
		public IDictionary<string,TokenMessage> TokenMessages { get; set; }
		
		public BatchInstanceInfo()
		{
			JobInfos = new ConcurrentDictionary<string, JobInstanceInfo>();
			TokenMessages = new ConcurrentDictionary<string, TokenMessage>();
		}
	}
}