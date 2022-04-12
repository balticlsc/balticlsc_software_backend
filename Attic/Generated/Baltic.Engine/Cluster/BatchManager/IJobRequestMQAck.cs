///////////////////////////////////////////////////////////
//  IJobRequestMQ.cs
//  Implementation of the Interface IJobRequestMQ
//  Generated by Enterprise Architect
//  Created on:      21-mar-2020 20:59:12
//  Original author: smialek
///////////////////////////////////////////////////////////


using Baltic.Engine.Cluster;

namespace Baltic.Engine.Cluster.BatchManager
{
	public interface IJobRequestMQAck {

		/// 
		/// <param name="queueUid"></param>
		/// <param name="qc"></param>
		short RegisterWithQueue(string queueUid, IQueueConsumer qc);

		/// 
		/// <param name="queueUid"></param>
		/// <param name="jobMsgUid"></param>
		short AckJobMessage(string queueUid, string jobMsgUid);
	}
}//end IJobRequestMQ