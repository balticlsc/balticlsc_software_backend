///////////////////////////////////////////////////////////
//  NetworkRegistry.cs
//  Implementation of the Class NetworkRegistry
//  Generated by Enterprise Architect
//  Created on:      16-mar-2020 20:57:16
//  Original author: smialek
///////////////////////////////////////////////////////////


using System.Collections.Generic;
using Baltic.DataModel.Execution;
using Baltic.DataModel.Resources;

namespace Baltic.Database.NetworkRegistry {
	public class NetworkBrokerageProxy : INetworkBrokerage {

		public NetworkBrokerageProxy(){

		}

		~NetworkBrokerageProxy(){

		}

		/// 
		/// <param name="range"></param>
		public List<CCluster> GetMatchingClusters(ResourceReservationRange range){
			List<CCluster> cl = new List<CCluster>();
			cl.Add(new CCluster());
			return cl;
		}

	}//end NetworkBrokerageProxy

}//end namespace Baltic.Database.NetworkRegistry