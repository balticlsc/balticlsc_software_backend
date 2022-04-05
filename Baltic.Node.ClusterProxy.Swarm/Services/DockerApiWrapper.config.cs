using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Baltic.Types.Protos;
using Docker.DotNet;
using Docker.DotNet.Models;
using Newtonsoft.Json.Linq;
using Serilog;

namespace Baltic.Node.ClusterProxy.Swarm.Services
{
    public partial class DockerApiWrapper
    {
        public async Task<string> ConfigCreate(XConfigFileDescription configDescription, string moduleId, string batchId)
        {
            var configName = DockerSwarmHelper.CreateConfigName(configDescription, moduleId, batchId);
            Log.Information($"Config file: {configDescription.MountPath} for module: {moduleId} will have following name {configName}");
            Log.Information($"Attempt to create config file: {configName}");
            var labels = new Dictionary<string, string>
            {
                {DockerStackLabel, batchId},
                {BalticModuleLabel, moduleId}
            };

            var configSpec = new ConfigSpec()
            {
                Name = configName,
                Data = configDescription.Data.ToEncodedConfigData(),
                Labels = labels,
            };

            var configParameters = new SwarmCreateConfigParameters()
            {
                Config = configSpec
            };
                
            var config = await ConfigGetById(configName);
            if (config != null)
            {
                if (config.Spec.Data == configDescription.Data.ToEncodedConfigData())
                {
                    Log.Information($"Exact same configuration file already exists in docker. Creation of {configName} was skipped.");
                    return config.ID;
                }

                var message =
                    $"Configuration file with name {configName} already exists in docker but with different data.";
                Log.Error(message);
                throw new Exception(message);
            }
            
            try
            {
                var createResponse = await _dockerClient.Swarm.CreateConfigAsync(configParameters);
                Log.Information($"Docker client created config with name: {configSpec.Name} and ID: {createResponse.Id}");
                return createResponse.Id;
            }
            catch (DockerApiException e)
            {
                var errorMessage = (string) JObject.Parse(e.ResponseBody)["message"];
                Log.Error($"Docker API return: {errorMessage}");
                throw;
            }
            catch (Exception e)
            {
                Log.Error($"Unknown exception was thrown: \n {e.Message}");
                throw;
            }
        }
        
        public async Task<SwarmConfig> ConfigGetById(string configId)
        {
            Log.Information($"Attempt to find config with name: {configId}");
            try
            {
                var configs = await _dockerClient.Swarm.ListConfigsAsync();
                var foundConfig = configs.SingleOrDefault(c => c.Spec.Name == configId);
                Log.Information(foundConfig == null
                    ? $"Docker client did not find config with name: {configId}"
                    : $"Docker client found config with name: {configId} and ID: {foundConfig?.ID}");
                return foundConfig;
            }
            catch (DockerApiException e)
            {
                var errorMessage = (string) JObject.Parse(e.ResponseBody)["message"];
                Log.Error($"Docker API return: {errorMessage}");
                throw;
            }
            catch (Exception e)
            {
                Log.Error(e.Message);
                throw;
            }

        }

        public async Task<IEnumerable<SwarmConfig>> ConfigGetByModuleId(string moduleId)
        {
            Log.Information($"Attempt to find config related to module: {moduleId}");
            try
            {
                var configs = await _dockerClient.Swarm.ListConfigsAsync();
                var configGetByModuleId = configs.ToList();
                configGetByModuleId = configGetByModuleId.Where(c => (c.Spec.Labels.ContainsKey(BalticModuleLabel) && c.Spec.Labels[BalticModuleLabel] == moduleId)).ToList();
                if (configGetByModuleId.Any())
                {
                    Log.Information($"Docker client found configs related to module: {moduleId}");
                    return configGetByModuleId;
                }
                Log.Information($"Docker client did not found any configs related to module: {moduleId}");
                return configGetByModuleId;
            }
            catch (DockerApiException e)
            {
                var errorMessage = (string) JObject.Parse(e.ResponseBody)["message"];
                Log.Error($"Docker API return: {errorMessage}");
                throw;
            }
            catch (Exception e)
            {
                Log.Error(e.Message);
                throw;
            }
        }
        
        public async Task ConfigRemove(string configId)
        {
            Log.Information($"Attempt to remove config with name: {configId}");
            try
            {
                await _dockerClient.Swarm.RemoveConfigAsync(configId);
                Log.Information($"Docker client starts removing config with name: {configId}");
            }
            catch (Exception e)
            {
                Log.Error(e.InnerException?.Message);
                throw;
            }
        }
    }
}