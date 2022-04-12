/*
 * Created by SharpDevelop.
 * User: smialek
 * Date: 29.02.2020
 * Time: 21:36
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Threading;
using Baltic.DataModel.CAL;
using Baltic.DataModel.CALExecutable;
using Baltic.DataModel.Execution;

using Baltic.Database.UnitRegistry;
using Baltic.Database.TaskRegistry;
using Baltic.Database.NetworkRegistry;
using Baltic.Database.Entities;

using Baltic.Engine.TaskManager;
using Baltic.Engine.MultiQueue;
using Baltic.Engine.TaskProcessor;
using Baltic.Engine.JobBroker;

using Baltic.Engine.Cluster.BatchManager;
using Baltic.Engine.Cluster.Cluster;
using Baltic.Engine.Cluster.JobManager;

namespace BalticLSC
{
	class Program
	{
		
		// Master Node
		public static DataModelImplFactory dmf;
		public static TaskManager tm;
		public static UnitRegistryMock ar;
		public static TaskProcessingProxy tppx;
		public static TaskManagementProxy tmpx;
		public static TaskBrokerageProxy tbpx;
		public static TaskRegistryMock trmock;
		public static MultiQueueMock tmq;
		public static TaskProcessor tp;
		public static JobBroker jb;
		public static ClusterNodeAccessFactory af;
		public static NetworkBrokerageProxy nr;
		
		public static JobRequestMQAckAdapter jrmqa;
		public static TokenMQAdapter tmqa;
		public static TokenMQDirectProxy tmqd;
		public static JobRequestMQProxy jrm;
		
		// Cluster Node
		public static BatchManager bmgr;
		public static JobRequestMQAckProxy jrmq;
		public static ClusterProxy cprox;
		public static TokenMQProxy mq;
		
		public static void Main(string[] args)
		{
			Console.WriteLine("** BalticLSC Task Engine ***");
			
			// Master Node - init
			
			dmf = new DataModelImplFactory();
			tm = new TaskManager();
			ar = new UnitRegistryMock();
			tppx = new TaskProcessingProxy();
			tmpx = new TaskManagementProxy();
			tbpx = new TaskBrokerageProxy();
			trmock = new TaskRegistryMock();
			tmq = new MultiQueueMock();
			tp = new TaskProcessor();
			jb = new JobBroker();
			af = new ClusterNodeAccessFactory();
			nr = new NetworkBrokerageProxy();
			
			jrmqa = new JobRequestMQAckAdapter();
			tmqa = new TokenMQAdapter();
			tmqd = new TokenMQDirectProxy();
			jrm = new JobRequestMQProxy();
			
			tppx.Init(trmock);
			tmpx.Init(trmock);
			
			jb.Init(jrm,nr,tbpx,af,dmf);
			tp.Init(tmqd,tppx,jb);
			tm.Init(ar,tmpx,tp,dmf);
			
			jrmqa.Mq = tmq;
			tmqa.init(tp);
			tmqa.Mq = tmq;
			tmqd.Mq = tmq;
			jrm.Mq = tmq;
			
			// Cluster Node - init
			
			jrmq = new JobRequestMQAckProxy();
			cprox = new ClusterProxy();
			bmgr = new BatchManager();
			mq = new TokenMQProxy();
			
			bmgr.Init(jrmq,cprox);
			af.Ib = bmgr;
			jrmq.init(jrmqa);
			jrmq.Q = tmq; // MOCK
			mq.Init(tmqa);
			mq.Q = tmq; //MOCK
			cprox.Mq = mq; //MOCK
			
			// Start the MultiQueue in a separate thread (necessary for proper message delivery)
			
			Thread job_thread = new Thread(tmq.RunQueue);
			job_thread.Start();
			
			// Run an app
			
			TaskParameters par = new TaskParameters(){ClusterAllocation = UnitStrength.Weak};
			
			string task_uid = tm.InitiateTask("YetAnotherImageProcessor_x01y2",par);
			
			CDataSet ds = new CDataSet();
			ds.Name = "MyImage1";
			tm.InjectDataSet(task_uid,"Film01",ds);
			
			Thread.Sleep(23000);
			
			ds = new CDataSet();
			ds.Name = "MyOutputImage1";
			tm.InjectDataSet(task_uid,"Proc_Film01",ds);
			
			Console.WriteLine("==> Data Injected");
			Console.ReadKey(true);
		}
	}
}