///////////////////////////////////////////////////////////
//  JobRequestMQAckAPI.cs
//  Implementation of the Interface JobRequestMQAckAPI
//  Generated by Enterprise Architect
//  Created on:      02-kwi-2020 14:07:10
//  Original author: smialek
///////////////////////////////////////////////////////////




namespace Baltic.Engine.MultiQueue
{
	public interface JobRequestMQAckAPI {

		/// 
		/// <param name="queue_uid"></param>
		/// <param name="job_msg_uid"></param>
		short ackJobMessage(string queue_uid, string job_msg_uid);
	}
}//end JobRequestMQAckAPI