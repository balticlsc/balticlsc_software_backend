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
        public async Task<string> VolumeCreate(XVolumeDescription volumeDescription, string moduleId, string batchId)
        {
            var volumeName = DockerSwarmHelper.CreateVolumeName(volumeDescription, moduleId, batchId);
            Log.Information($"Volume: {volumeDescription.MountPath} for module: {moduleId} will have following name {volumeName}");
            Log.Information($"Attempt to create Volume: {volumeName}");
            var labels = new Dictionary<string, string>
            {
                {DockerStackLabel, batchId},
                {BalticModuleLabel, moduleId}
            };
            var volumesRestParameters = new VolumesCreateParameters()
            {
                Driver = "local",
                Labels = labels,
                Name = volumeName
            };

            // Docker api allows to create volume with the same name!?
            try
            {
                var volumeResponse = await _dockerClient.Volumes.CreateAsync(volumesRestParameters);
                Log.Information($"Docker client volume with name: {volumeResponse.Name}");
                return volumeResponse.Name;
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
        
        public async Task<VolumeResponse> VolumeGetById(string volumeId)
        {
            Log.Information($"Attempt to find Volume with name: {volumeId}");
            try
            {
                var volumes = await _dockerClient.Volumes.ListAsync();
                var foundVolume = volumes.Volumes.SingleOrDefault(v => v.Name == volumeId);
                Log.Information($"Docker client found Volume with name: {volumeId}");
                return foundVolume;
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
        
        public async Task<IEnumerable<VolumeResponse>> VolumeGetByModuleId(string moduleId)
        {
            Log.Information($"Attempt to find volumes related to module: {moduleId}");
            try
            {
                var volumes = await _dockerClient.Volumes.ListAsync();
                var foundVolumes = volumes.Volumes.Where(v => (v.Labels?.ContainsKey(BalticModuleLabel) ?? false) && v.Labels[BalticModuleLabel] == moduleId).ToList();
                if (foundVolumes.Any())
                {
                    Log.Information($"Docker client found volumes related to module: {moduleId}");
                    return foundVolumes;
                }
                Log.Information($"Docker client did not found any volumes related to module: {moduleId}");
                return foundVolumes;
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
        
        public async Task VolumeRemove(string volumeId)
        {
            Log.Information($"Attempt to remove Volume with name: {volumeId}");
            try
            {
                var volumeResponse = await VolumeGetById(volumeId);
                await _dockerClient.Volumes.RemoveAsync(volumeResponse.Name);
                Log.Information($"Docker client starts removing Volume with name: {volumeId}");
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
    }
}