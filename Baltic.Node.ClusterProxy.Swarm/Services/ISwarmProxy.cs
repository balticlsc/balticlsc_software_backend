using System.Collections.Generic;
using System.Threading.Tasks;
using Docker.DotNet.Models;
using Baltic.Types.Protos;

namespace Baltic.Node.ClusterProxy.Swarm.Services
{
    public interface ISwarmProxy
    {
        // Methods that wraps Docker REST API
        
        // Network related methods
        public Task<string> NetworkCreate(string networkId);
        public Task<NetworkResponse> NetworkGetById(string networkId);
        public Task NetworkRemove(string networkId);
        
        // ConfigFiles related methods
        public Task<string> ConfigCreate(XConfigFileDescription configDescription, string moduleId, string batchId);
        public Task<SwarmConfig> ConfigGetById(string configId);
        public Task<IEnumerable<SwarmConfig>> ConfigGetByModuleId(string moduleId);
        public Task ConfigRemove(string configId);
        
        // Volumes related methods
        public Task<string> VolumeCreate(XVolumeDescription volumeDescription, string moduleId, string batchId);
        public Task<VolumeResponse> VolumeGetById(string volumeId);
        public Task<IEnumerable<VolumeResponse>> VolumeGetByModuleId(string moduleId);
        public Task VolumeRemove(string volumeId);

        // Services related methods
        public Task<string> ServiceCreate(XBalticModuleBuild balticModuleBuild);
        public Task<SwarmService> ServiceGetById(string serviceId);
        public Task<IEnumerable<SwarmService>> ServiceGetByBatchId(string batchId);
        public Task ServiceRemove(string serviceId);
        
        // Containers related methods
        // public Task<IEnumerable<ContainerListResponse>> ContainerGetByModuleId(string moduleId);
    }
}