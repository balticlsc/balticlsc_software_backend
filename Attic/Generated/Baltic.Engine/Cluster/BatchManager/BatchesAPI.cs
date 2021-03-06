///////////////////////////////////////////////////////////
//  JobsAPI.cs
//  Implementation of the Interface JobsAPI
//  Generated by Enterprise Architect
//  Created on:      16-mar-2020 18:01:31
//  Original author: smialek
///////////////////////////////////////////////////////////


using Baltic.DataModel.CALExecutable;

namespace Baltic.Engine.Cluster.BatchManager
{
	public interface BatchesAPI {

		/// 
		/// <param name="bm"></param>
		short startJobBatch(BatchMessage bm);
	}
}//end JobsAPI