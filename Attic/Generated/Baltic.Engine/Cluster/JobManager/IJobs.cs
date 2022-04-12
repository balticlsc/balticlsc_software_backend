///////////////////////////////////////////////////////////
//  IJobs.cs
//  Implementation of the Interface IJobs
//  Generated by Enterprise Architect
//  Created on:      31-mar-2020 13:59:10
//  Original author: smialek
///////////////////////////////////////////////////////////


using System.Collections.Generic;
using Baltic.DataModel.CALExecutable;

namespace Baltic.Engine.Cluster.JobManager
{
	public interface IJobs  {

		///
		/// <param name="job"></param>
		/// <param name="jm"></param>
		short RegisterJob(IJob job, JobMessage jm);

		/// 
		/// <param name="depthLevel"></param>
		short SetBatchDepthLevel(int depthLevel);
	}
}//end IJobs