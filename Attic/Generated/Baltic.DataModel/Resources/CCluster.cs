///////////////////////////////////////////////////////////
//  CCluster.cs
//  Implementation of the Class CCluster
//  Generated by Enterprise Architect
//  Created on:      16-kwi-2020 15:51:11
//  Original author: smialek
///////////////////////////////////////////////////////////



using System.Collections.Generic;
using Baltic.DataModel.Resources;

namespace Baltic.DataModel.Resources {
	public class CCluster {

		public string Uid;
		public string Name;
		public string Country;
		public string Endpoint;
		public ClusterStatus Status;
		/// <summary>
		/// Probability that the Cluster stays active (0-100%)
		/// </summary>
		public short AvailabilityLevel;
		public ClusterPrivacy Privacy;
		public ClusterManifest Manifest;
		public List<CMachine> Machines;
		public List<ClusterMetric> Performance;

		public CCluster(){

		}

		~CCluster(){

		}

	}//end CCluster

}//end namespace Baltic.DataModel.Resources