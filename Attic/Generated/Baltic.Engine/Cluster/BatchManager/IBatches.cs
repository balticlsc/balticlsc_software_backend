///////////////////////////////////////////////////////////
//  IBatches.cs
//  Implementation of the Interface IBatches
//  Generated by Enterprise Architect
//  Created on:      31-mar-2020 16:44:18
//  Original author: smialek
///////////////////////////////////////////////////////////


using Baltic.DataModel.CALExecutable;

namespace Baltic.Engine.Cluster.BatchManager
{
	public interface IBatches {

		/// 
		/// <param name="bm"></param>
		short StartJobBatch(BatchMessage bm);
	}
}//end IBatches