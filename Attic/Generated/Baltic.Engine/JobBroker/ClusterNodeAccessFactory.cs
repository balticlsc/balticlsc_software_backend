///////////////////////////////////////////////////////////
//  JobsAPIFactory.cs
//  Implementation of the Class JobsAPIFactory
//  Generated by Enterprise Architect
//  Created on:      16-mar-2020 20:51:43
//  Original author: smialek
///////////////////////////////////////////////////////////


using Baltic.DataModel.Resources;
using Baltic.Engine.Cluster.BatchManager;

namespace Baltic.Engine.JobBroker
{
	public class ClusterNodeAccessFactory : IClusterNodeAccessFactory{

		public IBatches Ib; // MOCK!!
		
		public ClusterNodeAccessFactory(){

		}

		~ClusterNodeAccessFactory(){

		}

		/// 
		/// <param name="cluster"></param>
		public IBatches CreateClusterNodeAccess(CCluster cluster){
			return Ib; // MOCK!!!
		}

	}//end ClusterNodeAccessFactory
}