using System.Threading;
using Baltic.Core.Utils;
using Baltic.DataModel.CAL.Types;
using Baltic.DataModel.CALExecutable;
using Baltic.DataModel.Execution;
using Baltic.DiagramRegistry.DataAccess;
using Baltic.Engine.JobBroker;
using Baltic.Engine.TaskProcessor;
using Baltic.Engine.TaskManager;
using Baltic.NetworkRegistry.DataAccess;
using Baltic.Node.BatchManager.Proxies;
using Baltic.Node.Engine.BatchManager;
using Baltic.Queue.MultiQueue;
using Baltic.TaskManager.Proxies;
using Baltic.TaskRegistry.DataAccess;
using Baltic.Types.Entities;
using Baltic.UnitRegistry.DataAccess;
using Microsoft.Extensions.Configuration;
using Serilog;

namespace Test.Engine
{
    class Program
	{
		
		// Master Node
		public static DataModelImplFactory dmf;
		public static TaskManager tm;
		public static UnitRegistryMock ur;
		public static UnitProcessingDaoImplMock ar;
		public static DiagramDaoImplMock dr;
		public static TaskProcessingDaoImplMock tppx;
		public static TaskManagementDaoImplMock tmpx;
		public static TaskBrokerageDaoImplMock tbpx;
		public static TaskRegistryMock trmock;
		public static MultiQueueDbMock tmq;
		public static TaskProcessor tp;
		public static JobBroker jb;
		public static ClusterNodeAccessFactory af;
		public static NetworkRegistryMock nr;
		public static NetworkBrokerageDaoImplMock nrm;

		public static MultiQueue mq;
		
		// Cluster Node
		public static BatchManager bmgr;
		public static BatchManagerDbMock bmgrDb;
		public static BalticServerProxy jrmq;
		public static ClusterProxyProxyMock cprox;

		public static IConfiguration configuration = null;

		public static void Main(string[] args)
		{
			LoggerSetup.CreateLogger(true, "C:/logs/baltic-test-engine-{Date}.txt");
			Log.Debug("** BalticLSC Task Engine Starting ***");
			
			dmf = new DataModelImplFactory();
			
			ur = new UnitRegistryMock();
			ar = new UnitProcessingDaoImplMock(ur);
			dr = new DiagramDaoImplMock();

			trmock = new TaskRegistryMock();
			tppx = new TaskProcessingDaoImplMock(trmock);
			tmpx = new TaskManagementDaoImplMock(trmock);
			tbpx = new TaskBrokerageDaoImplMock(trmock);
			
			nr = new NetworkRegistryMock();
			nrm = new NetworkBrokerageDaoImplMock(nr);
			
			cprox = new ClusterProxyProxyMock();
			bmgrDb = new BatchManagerDbMock();

			tmq = new MultiQueueDbMock();
			mq = new MultiQueue(tmq,configuration);
			
			//tmqa = new BalticServerController();
			
			//jrmq = new BalticServerProxy(tmqa,tmq);
			bmgr = new BatchManager(jrmq,cprox,bmgrDb,configuration);
			//af = new ClusterNodeAccessFactory(bmgr);
			
			jb = new JobBroker(mq,nrm,tbpx,af,dmf,configuration);
			tp = new TaskProcessor(mq,tppx,jb,configuration);
			tm = new TaskManager(ar,tmpx,tp,dmf,dr,jb);

			// Run an app
			
			TaskParameters par = new TaskParameters(){ClusterAllocation = UnitStrength.Weak};
			
			string taskUid = tm.InitiateTask("YetAnotherImageProcessor_rel_001",par,"user1"); 
			
			//string taskUid = tm.InitiateAppTestTask("YetAnotherImageProcessor_001",par,"user1");

			CDataSet ds = new CDataSet();
			ds.Values = "MyImage1";
			tm.InjectDataSet(taskUid,"Film01",ds);
			
			Thread.Sleep(23000);
			
			ds = new CDataSet();
			ds.Values = "MyOutputImage1";
			tm.InjectDataSet(taskUid,"Proc_Film01",ds);
			
			Log.Debug("==> Data Injected");
			// Console.ReadKey(true);
		}
	}
}