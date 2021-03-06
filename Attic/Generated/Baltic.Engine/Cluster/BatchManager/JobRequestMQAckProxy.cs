///////////////////////////////////////////////////////////
//  JobRequestMQ.cs
//  Implementation of the Class JobRequestMQ
//  Generated by Enterprise Architect
//  Created on:      21-mar-2020 21:04:21
//  Original author: smialek
///////////////////////////////////////////////////////////


using Baltic.Engine.Cluster;
using Baltic.Engine.MultiQueue;

namespace Baltic.Engine.Cluster.BatchManager
{
	public class JobRequestMQAckProxy : IJobRequestMQAck {

		private JobRequestMQAckAPI jmq;
	
		// MOCK
		public MultiQueueMock Q;
		// MOCK

		public JobRequestMQAckProxy(){

		}

		~JobRequestMQAckProxy(){

		}
	
		/// 
		/// <param name="api"></param>
		public void init(JobRequestMQAckAPI api){
			jmq = api;
		}

		/// 
		/// <param name="queueUid"></param>
		/// <param name="qc"></param>
		public short RegisterWithQueue(string queueUid, IQueueConsumer qc){
			Q.Consumers[queueUid] = qc;
			return 0;
		}

		/// 
		/// <param name="queueUid"></param>
		/// <param name="jobMsgUid"></param>
		public short AckJobMessage(string queueUid, string jobMsgUid){
			return jmq.ackJobMessage(queueUid,jobMsgUid);
		}

	}
}//end JobRequestMQ