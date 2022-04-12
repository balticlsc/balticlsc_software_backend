using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.RegularExpressions;
using Baltic.DataModel.CALMessages;
using Serilog;
using BalticModuleBuild = Baltic.DataModel.CALMessages.BalticModuleBuild;

namespace Baltic.Node.Engine.BatchManager {
	public class AccessUtility {
		
		public const string ModuleManagerId = "mm-";
		public const string ModuleManagerPort = "7300";
		public static string MongoConnectionstring { get; set; }

		public static void SetMongoDbConnectionstring(string connectionstring)
		{
			MongoConnectionstring = connectionstring;
		}
		/// 
		/// <param name="serviceBuilds"></param>
		public static Dictionary<string,string> GetStorageAccess(List<BalticModuleBuild> serviceBuilds){
			Dictionary<string,string> ret = new Dictionary<string, string>();

			// MOCK
			ret.Add("MongoDB", JsonSerializer.Serialize(new
			{
				connectionstring = MongoConnectionstring,
				// connectionstring = "mongodb://someuser:somepass@host.docker.internal:27017",
				Table = "images"
			}));
			// MOCK
			
			return ret;
		}

		/// 
		/// <param name="build"></param>
		/// <param name="jobInstanceUid"></param>
		/// <param name="batchInstanceUid"></param>
		/// <param name="storageAccess"></param>
		/// <param name="clusterProjectName"></param>
		public static BalticModuleBuild AddEnvironmentToJobBuild(BalticModuleBuild build, string jobInstanceUid, 
			string batchInstanceUid, Dictionary<string,string> storageAccess, string clusterProjectName)
		{
			build.EnvironmentVariables.Add("SYS_MODULE_INSTANCE_UID",jobInstanceUid);
			build.EnvironmentVariables.Add("SYS_BATCH_MANAGER_TOKEN_ENDPOINT",
				$"http://{ModuleManagerId}{batchInstanceUid}.{clusterProjectName}-{batchInstanceUid}:{ModuleManagerPort}/token");
			build.EnvironmentVariables.Add("SYS_BATCH_MANAGER_ACK_ENDPOINT",
				$"http://{ModuleManagerId}{batchInstanceUid}.{clusterProjectName}-{batchInstanceUid}:{ModuleManagerPort}/ack");

			// TODO - clean - remove "Inject the config file into the build"
			/* ConfigFileDescription pinsConfigFile = build.ConfigFiles.Find(f => PinsConfig.PinsConfigMountPath == f.MountPath);
			if (null != pinsConfigFile) // should always happen
			{
				List<PinsConfig> data = new List<PinsConfig>();
				List<string> pinsConfigFiles = SplitConfigFile(pinsConfigFile.Data);
				Log.Debug($"Config split into {pinsConfigFiles.Count} pins");
				foreach (string pinFile in pinsConfigFiles)
				{
					PinsConfig pinsConfig = PinsConfig.Parse(pinFile);
					Log.Debug($"Parsed config file for pin in Job: {build.Image}\n" +
					          $"{pinsConfig}");
					data.Add(pinsConfig);
					if (!string.IsNullOrEmpty(pinsConfig.AccessCredential))
						continue;
					if (!storageAccess.ContainsKey(pinsConfig.AccessType))
					{
						Log.Error($"No credentials for the Pin: {pinsConfig.PinName} in Job Instance {jobInstanceUid}");
						continue; // should not happen
					}
					pinsConfig.AccessCredential = storageAccess[pinsConfig.AccessType];
					Log.Debug($"Updated config file for pin in Job: {build.Image}\n" +
					          $"{pinsConfig}");}
				pinsConfigFile.Data = "[" + string.Join(",\n",data) + "]";
			}
			Log.Debug($"Updated config file for module {jobInstanceUid}:\n{pinsConfigFile?.Data}");
			*/
			return build;
		}

		private static List<string> SplitConfigFile(string data)
		{
			List<string> ret = new List<string>();
			int index = -1, bracketCounter = 0;
			for (int i = 0; i < data.Length; i++)
			{
				if ('{' == data[i])
				{
					if (0 == bracketCounter)
						index = i;
					bracketCounter++;
				}
				else if ('}' == data[i])
				{
					bracketCounter--;
					if (0 == bracketCounter)
						ret.Add(data.Substring(index,i-index+1));
				}
			}
			return ret;
		}
	}
}