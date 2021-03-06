///////////////////////////////////////////////////////////
//  IClusterProxy.cs
//  Implementation of the Interface IClusterProxy
//  Generated by Enterprise Architect
//  Created on:      21-mar-2020 20:42:18
//  Original author: smialek
///////////////////////////////////////////////////////////


using Baltic.Engine.Cluster.JobManager;

namespace Baltic.Engine.Cluster.Cluster
{
	public interface ICluster {


		/// 
		/// <param name="YAML"></param>
		IJob StartJob(string YAML);

		/// 
		/// <param name="YAML"></param>
		IJobs StartBatch(string YAML);

	}
}//end ICluster