using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;

namespace Baltic.Queue.MultiQueue
{
	public class MultiQueueDbMock {

		/// <summary>
		/// string - TaskUid
		/// </summary>
		public ConcurrentDictionary<string,List<MsgQueueFamily>> _queueFamilies;

		public ConcurrentDictionary<string, SemaphoreSlim> _taskSemaphores;
		
		public MultiQueueDbMock()
		{
			_queueFamilies = new ConcurrentDictionary<string, List<MsgQueueFamily>>();
			_taskSemaphores = new ConcurrentDictionary<string, SemaphoreSlim>();
		}

	}
}