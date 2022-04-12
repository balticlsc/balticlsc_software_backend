///////////////////////////////////////////////////////////
//  ITokenMQ.cs
//  Implementation of the Interface ITokenMQ
//  Generated by Enterprise Architect
//  Created on:      23-mar-2020 09:58:52
//  Original author: smialek
///////////////////////////////////////////////////////////


using Baltic.DataModel.CALExecutable;
using Baltic.Engine.Cluster;

namespace Baltic.Engine.Cluster.JobManager
{
	public interface ITokenMQ {

		/// 
		/// <param name="queueUid"></param>
		/// <param name="qc"></param>
		short RegisterWithQueue(string queueUid, IQueueConsumer qc);

		/// 
		/// <param name="tm"></param>
		short PutTokenMessage(TokenMessage tm);

		/// 
		/// <param name="queueUid"></param>
		/// <param name="tm"></param>
		short AckTokenMessage(string queueUid, TokenMessage tm);

	}
}//end ITokenMQ