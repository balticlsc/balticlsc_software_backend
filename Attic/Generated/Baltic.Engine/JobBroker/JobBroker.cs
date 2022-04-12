///////////////////////////////////////////////////////////
//  JobBroker.cs
//  Implementation of the Class JobBroker
//  Generated by Enterprise Architect
//  Created on:      02-mar-2020 11:26:09
//  Original author: smialek
///////////////////////////////////////////////////////////


using System;
using System.Collections.Generic;
using Baltic.DataModel.CALExecutable;
using Baltic.DataModel.Execution;
using Baltic.DataModel.Resources;
using Baltic.Engine.Cluster.BatchManager;
using Baltic.Engine.MultiQueue;
using Baltic.Engine.TaskProcessor;
using Baltic.Database.TaskRegistry;
using Baltic.Database.NetworkRegistry;
using Baltic.Database.Entities;

namespace Baltic.Engine.JobBroker
{
	public class JobBroker : IJobBroker
	{
		private IJobRequestMQ Queue;
		private INetworkBrokerage NetworkRegistry;
		private ITaskBrokerage TaskRegistry;
		private IDataModelImplFactory Factory;
		
		private IClusterNodeAccessFactory AccessFactory;

		public JobBroker()
		{

		}

		~JobBroker()
		{

		}
	
		///
		/// <param name="q"></param>
		/// <param name="nr"></param>
		/// <param name="tr"></param>
		/// <param name="af"></param>
		public void Init(IJobRequestMQ q, INetworkBrokerage nr, ITaskBrokerage tr, IClusterNodeAccessFactory af, IDataModelImplFactory dmf){
			Queue = q; NetworkRegistry = nr; TaskRegistry = tr; AccessFactory = af; Factory = dmf;
		}
	
		/// 
		/// <param name="batch"></param>
		/// <param name="bm"></param>
		/// <param name="reservationRange"></param>
		public short ActivateJobBatch(CJobBatch batch, BatchMessage bm, ResourceReservationRange reservationRange)
		{
			//*test*
			Console.WriteLine(ConsoleString() +  " ActivateJobBatch START: " + bm.MsgUid + "\n## " + batch);
			//*test*
		
			if (0 > Queue.CheckQueue(bm.JobsQueueUid)){
				List<CCluster> lc;
				lc = NetworkRegistry.GetMatchingClusters(reservationRange); // TODO - przepropagować
				lc = SortClusters(lc);
			
				Queue.PutMessage(bm.JobsQueueUid,null);
			
				foreach (CCluster cl in lc) {	
					IBatches ja = AccessFactory.CreateClusterNodeAccess(cl);
					if (0 == ja.StartJobBatch(bm)) {
						BatchExecution be = Factory.CretateBatchExecution();
						be.Cluster = cl;
						be.Start = DateTime.Now;
						TaskRegistry.AddBatchExecution(batch,be);
						//*test*
						Console.WriteLine(ConsoleString() + " ActivateJobBatch FINISH OK: " + bm.MsgUid);
						//*test*
						return 0;
					}
				}
			}
		
			//*test*
			Console.WriteLine(ConsoleString() + " ActivateJobBatch FINISH ERR: " + bm.MsgUid);
			//*test*
		
			return -1;
		}

		/// 
		/// <param name="batch"></param>
		/// <param name="jm"></param>
		public short ActivateJob(CJobBatch batch, JobMessage jm)
		{
			//*test*
			Console.WriteLine(ConsoleString() + " ActivateJob START: " + jm.MsgUid + "\n## " + jm);
			//*test*
		
			IEnumerator<string> index = jm.RequiredPinQueues.Keys.GetEnumerator();
			index.MoveNext();
		
			string queue_uid = TaskMessageUtility.GetBatchQueueNameFromJob(jm.RequiredPinQueues[index.Current],batch);
		
			Queue.PutMessage(queue_uid,jm);
		
			//*test*
			Console.WriteLine(ConsoleString() + " Activate Job FINISH: " + jm.MsgUid);
			//*test*
		
			return 0;
		}

		/// 
		/// <param name="lc"></param>
		public List<CCluster> SortClusters(List<CCluster> lc){
			return lc;
		}
	
		private string ConsoleString(){
			return "## JobBroker " + DateTime.Now.ToString("dd.MM.yyyy HH:mm:ss.ffff") + " ## ";
		}

	}
}//end JobBroker
