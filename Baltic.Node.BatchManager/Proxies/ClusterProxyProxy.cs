using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;
using System.Threading;
using Baltic.DataModel.Execution;
using Baltic.Node.Engine.BatchManager;
using Baltic.Node.Engine.DataAccess;
using Baltic.Types.Entities;
using Baltic.Types.Protos;
using Grpc.Net.Client;
using Microsoft.Extensions.Configuration;
using Serilog;
using BalticModuleBuild = Baltic.DataModel.CALMessages.BalticModuleBuild;

namespace Baltic.Node.BatchManager.Proxies
{
	public class ClusterProxyProxy : ICluster {
		
		private readonly string _address;
		private readonly string _clusterProjectName;
		private readonly GrpcChannelOptions _options;
		private GrpcChannel _channel;
		private ClusterProxy.ClusterProxyClient _cluster;
		private IDataModelImplFactory _factory;
		private bool _localEndpoint = false;
		
		private ClusterProxy.ClusterProxyClient Cluster
		{
			get
			{
				AppContext.SetSwitch("System.Net.Http.SocketsHttpHandler.Http2UnencryptedSupport", true);
				if (null == _channel)
				{
					Log.Information($"gRPC channel use address: {_address}");
					_channel = GrpcChannel.ForAddress(_address, _options);
					_cluster = new ClusterProxy.ClusterProxyClient(_channel);
				}
				else
				// TODO - check if the channel responds and initiate again if necessary
				if (null == _cluster)
					_cluster = new ClusterProxy.ClusterProxyClient(_channel);
				return _cluster;
			}
		}

		public ClusterProxyProxy(string address, IDataModelImplFactory factory, GrpcChannelOptions options, IConfiguration configuration)
		{
			// TODO - remove/update (is it used? which constructor was temporary?)
			_address = address;
			_clusterProjectName = configuration["clusterProjectName"];
			_options = options;
			_factory = factory;
			_localEndpoint = "true" == configuration["localExecution"];
			if (_localEndpoint) 
				Log.Warning("Batch Manager will be accessing Module Manager in LOCAL mode (http://localhost:7300)");
		}

		public ClusterProxyProxy(IDataModelImplFactory factory, IConfiguration configuration)
		{
			// _address = "https://localhost:6001/";
			var httpHandler = new HttpClientHandler
			{
				ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator
			};
			_options = new GrpcChannelOptions {HttpHandler = httpHandler};
			_address = configuration["clusterProxyUrl"];
			_clusterProjectName = configuration["clusterProjectName"];
			// _options = new GrpcChannelOptions();
			_factory = factory;
			_localEndpoint = "true" == configuration["localExecution"];
			if (_localEndpoint) 
				Log.Warning("Batch Manager will be accessing Module Manager in LOCAL mode (http://localhost:7300)");
		}

		/// 
		/// <param name="build"></param>
		/// <param name="jobInstanceUid"></param>
		/// <param name="batchInstanceUid"></param>
		public IJob StartJob(BalticModuleBuild build, string batchInstanceUid, string jobInstanceUid = null)
		{
			XBalticModuleBuild xBuild = new XBalticModuleBuild(build,batchInstanceUid,jobInstanceUid);
			Log.Debug($"{ConsoleString()} Attempting to start Job Instance: {xBuild.ModuleId} with build:\n" +
			          $"{xBuild}");
			
			Module id = new Module() {BatchId = batchInstanceUid, ModuleId = xBuild.ModuleId};
			ClusterStatusResponse response = Cluster.RunBalticModule(xBuild);
			int k = 1;
			do
			{
				Thread.Sleep(1000);
				Log.Debug($"{ConsoleString()} Waiting for Job Instance ({xBuild.ModuleId}) to be active... [{k++}]");
				response = Cluster.CheckBalticModuleStatus(id);
				if (ClusterStatusResponse.Types.StatusCode.Error == response.Status && "Deployment does not have minimum availability." != response.Message || k > 120)
				{
					Log.Error($"{ConsoleString()} Job Instance {xBuild.ModuleId} could not be started properly " +
					          (k>120 ? "(Timeout)" : $"(Message: {response.Message})"));
					Cluster.DisposeBalticModule(id);
					Log.Debug($"{ConsoleString()} Workspace {batchInstanceUid} purged (or was not created)");
					return null;
				}
			} while (ClusterStatusResponse.Types.StatusCode.Active != response.Status);

			string url = _localEndpoint ? 
				"http://localhost:7300" : 
				$"http://{AccessUtility.ModuleManagerId}{batchInstanceUid}.{_clusterProjectName}-{batchInstanceUid}:{AccessUtility.ModuleManagerPort}";
			JobProxy job = new JobProxy(url, xBuild.ModuleId);
			return job;
		}

		/// 
		/// <param name="batchInstanceUid"></param>
		/// <param name="quota"></param>
		/// <param name="serviceBuilds"></param>
		public short StartBatch(string batchInstanceUid, ResourceReservation quota, List<BalticModuleBuild> serviceBuilds)
		{
			Log.Debug($"{ConsoleString()} Attempting to create Batch Instance: {batchInstanceUid} with {serviceBuilds.Count} service(s)");
			XWorkspace workspace = new XWorkspace()
			{
				BatchId = batchInstanceUid,
				Quota = new XWorkspaceQuota()
				{
					Cpus = quota.Cpus,
					Gpus = quota.Gpus,
					Memory = quota.Memory,
					Storage = quota.Storage
				}
			};
			ClusterStatusResponse response = Cluster.PrepareWorkspace(workspace);
			int k = 1;
			do
			{
				Thread.Sleep(1000);
				Log.Debug($"{ConsoleString()} Wait for Batch Instance to be active... [current status is {response.Status} try no.: {k++}]");
				response = Cluster.CheckWorkspaceStatus(new BatchId() {Id = batchInstanceUid});
				if (ClusterStatusResponse.Types.StatusCode.Error == response.Status || k > 120)
				{
					Log.Error($"{ConsoleString()} Cluster cannot prepare Batch Instance {batchInstanceUid} " +
					          (k>120 ? "(Timeout)" : $"(Message: {response.Message})"));
					Cluster.PurgeWorkspace(new BatchId() {Id = batchInstanceUid});
					Log.Debug($"{ConsoleString()} Batch Instance {batchInstanceUid} purged (or was not created)");
					return -1;
				}
			} while (ClusterStatusResponse.Types.StatusCode.Active != response.Status);
			Log.Debug($"{ConsoleString()} Batch Instance {batchInstanceUid} activated successfully");
			
			if (0 != serviceBuilds.Count)
				Log.Debug($"{ConsoleString()} Activating services for Batch Instance {batchInstanceUid}");

			foreach (BalticModuleBuild build in serviceBuilds) // TODO - start the service's jobs (and module manager - see below) asynchronously (in parallel)
				StartJob(build, batchInstanceUid);

			// Start a ModuleManager in the just started BatchInstance
			Log.Debug($"{ConsoleString()} Activating module manager for Batch Instance {batchInstanceUid}");
			StartJob(_factory.CreateModuleManagerBuild(), batchInstanceUid, "mm-" + batchInstanceUid);
			return 0;
		}

		public Dictionary<string, ResourceUsage> GetCurrentResourceUsage(string batchInstanceUid)
		{
			throw new NotImplementedException();
		}

		public Dictionary<string, ResourceUsage> GetRangeResourceUsage(string batchInstanceId, DateTime startTime, DateTime endTime)
		{
			throw new NotImplementedException();
		}

		public short StopJob(string jobInstanceUid, string batchInstanceUid)
		{
			Module id = new Module() {BatchId = batchInstanceUid, ModuleId = jobInstanceUid};
			ClusterStatusResponse response = Cluster.DisposeBalticModule(id);
			// TODO
			/*if (ClusterStatusResponse.Types.StatusCode.NotFound != response.Status)
				while (ClusterStatusResponse.Types.StatusCode.Active ==
				       (response = Cluster.CheckBalticModuleStatus(id)).Status)
				{
					Thread.Sleep(100);
				}*/
			
			return ClusterStatusResponse.Types.StatusCode.NotFound == response.Status ? (short)0 : (short)-1;
		}

		public short StopBatch(string batchInstanceUid)
		{
			BatchId id = new BatchId() {Id = batchInstanceUid};
			ClusterStatusResponse response = Cluster.PurgeWorkspace(id);
			// TODO
			/*if (ClusterStatusResponse.Types.StatusCode.NotFound != response.Status)
				while (ClusterStatusResponse.Types.StatusCode.Active ==
				       (response = Cluster.CheckWorkspaceStatus(id)).Status)
				{
					Thread.Sleep(100);
				}*/
			
			return ClusterStatusResponse.Types.StatusCode.NotFound == response.Status ? (short)0 : (short)-1;
		}
		
		private string ConsoleString(){
			return "## NODE.PROXY ## " + DateTime.Now.ToString("dd.MM.yyyy HH:mm:ss.ffff") + " ## ";
		}
	}
}