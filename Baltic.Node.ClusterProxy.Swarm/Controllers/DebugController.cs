#if DEBUG

using System;
using System.Threading.Tasks;
using Baltic.Node.ClusterProxy.Swarm.Services;
using Baltic.Types.Protos;
using Docker.DotNet;
using Grpc.Core;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;

namespace Baltic.Node.ClusterProxy.Swarm.Controllers
{
    
    [ApiController]
    [Route("/[controller]/")]  
    public class DebugController : Controller
    {
        private readonly ISwarmProxy _swarmProxy;
        private readonly IPortainerApi _portainerApi;
        private readonly SwarmClusterProxyService _clusterProxy;
        
        public DebugController(ISwarmProxy swarmProxy, IPortainerApi portainerApi)
        {
            _swarmProxy = swarmProxy;
            _portainerApi = portainerApi;
            _clusterProxy = new SwarmClusterProxyService(swarmProxy, portainerApi);
        }
        
        [HttpPost]
        [Route("Network/{networkId}")]  
        public async Task<ActionResult<string>> NetworkCreate([FromRoute] string networkId)
        {
            try
            {
                return Newtonsoft.Json.JsonConvert.SerializeObject(
                    new {
                        Success = true,
                        Data = await _swarmProxy.NetworkCreate(networkId) 
                        }, Newtonsoft.Json.Formatting.Indented);

            }
            catch (DockerApiException e)
            {
                return Newtonsoft.Json.JsonConvert.SerializeObject(
                    new {
                        Success = false,
                        ErrorType = "DockerApiException",
                        Error = e.Message
                    }, Newtonsoft.Json.Formatting.Indented);
            }
            catch (Exception e)
            {
                return Newtonsoft.Json.JsonConvert.SerializeObject(
                    new {
                        Success = false,
                        ErrorType = "General",
                        Error = e.Message
                    }, Newtonsoft.Json.Formatting.Indented);
            }
        }
        
        [HttpGet]
        [Route("Network/{networkId}")]  
        public async Task<ActionResult<string>> NetworkGetById([FromRoute] string networkId)
        {
            try
            {
                return Newtonsoft.Json.JsonConvert.SerializeObject(
                    new {
                        Success = true,
                        Data = await _swarmProxy.NetworkGetById(networkId) 
                    }, Newtonsoft.Json.Formatting.Indented);

            }
            catch (DockerApiException e)
            {
                return Newtonsoft.Json.JsonConvert.SerializeObject(
                    new {
                        Success = false,
                        ErrorType = "DockerApiException",
                        Error = e.Message
                    }, Newtonsoft.Json.Formatting.Indented);
            }
            catch (Exception e)
            {
                return Newtonsoft.Json.JsonConvert.SerializeObject(
                    new {
                        Success = false,
                        ErrorType = "General",
                        Error = e.Message
                    }, Newtonsoft.Json.Formatting.Indented);
            }
        }
        
        [HttpDelete]
        [Route("Network/{networkId}")]  
        public async Task<ActionResult<string>> NetworkRemove([FromRoute] string networkId)
        {
            try
            {
                await _swarmProxy.NetworkRemove(networkId);
                return Newtonsoft.Json.JsonConvert.SerializeObject(
                    new {
                        Success = true
                    }, Newtonsoft.Json.Formatting.Indented);

            }
            catch (DockerApiException e)
            {
                return Newtonsoft.Json.JsonConvert.SerializeObject(
                    new {
                        Success = false,
                        ErrorType = "DockerApiException",
                        Error = e.Message
                    }, Newtonsoft.Json.Formatting.Indented);
            }
            catch (Exception e)
            {
                return Newtonsoft.Json.JsonConvert.SerializeObject(
                    new {
                        Success = false,
                        ErrorType = "General",
                        Error = e.Message
                    }, Newtonsoft.Json.Formatting.Indented);
            }
        }
        
        
        [HttpPost]
        [Route("Config/{batchId}/{moduleId}")]
        public async Task<ActionResult<string>> ConfigCreate(
            [FromRoute] string batchId, 
            [FromRoute] string moduleId, 
            [FromBody] XConfigFileDescription configDescription)
        {
            try
            {
                return Newtonsoft.Json.JsonConvert.SerializeObject(
                    new {
                        Success = true,
                        Data = await _swarmProxy.ConfigCreate(configDescription, moduleId, batchId) 
                    }, Newtonsoft.Json.Formatting.Indented);

            }
            catch (DockerApiException e)
            {
                return Newtonsoft.Json.JsonConvert.SerializeObject(
                    new {
                        Success = false,
                        ErrorType = "DockerApiException",
                        Error = e.Message
                    }, Newtonsoft.Json.Formatting.Indented);
            }
            catch (Exception e)
            {
                return Newtonsoft.Json.JsonConvert.SerializeObject(
                    new {
                        Success = false,
                        ErrorType = "General",
                        Error = e.Message
                    }, Newtonsoft.Json.Formatting.Indented);
            }

        }  
        
        [HttpGet]
        [Route("Config/{configId}")]
        public async Task<ActionResult<string>> ConfigGetById([FromRoute] string configId)
        {
            try
            {
                return Newtonsoft.Json.JsonConvert.SerializeObject(
                    new {
                        Success = true,
                        Data = await _swarmProxy.ConfigGetById(configId) 
                    }, Newtonsoft.Json.Formatting.Indented);

            }
            catch (DockerApiException e)
            {
                return Newtonsoft.Json.JsonConvert.SerializeObject(
                    new {
                        Success = false,
                        ErrorType = "DockerApiException",
                        Error = e.Message
                    }, Newtonsoft.Json.Formatting.Indented);
            }
            catch (Exception e)
            {
                return Newtonsoft.Json.JsonConvert.SerializeObject(
                    new {
                        Success = false,
                        ErrorType = "General",
                        Error = e.Message
                    }, Newtonsoft.Json.Formatting.Indented);
            }
        }
        
        [HttpGet]
        [Route("Config/GetByModule/{moduleId}")]
        public async Task<ActionResult<string>> ConfigGetByModuleId([FromRoute] string moduleId)
        {
            try
            {
                return Newtonsoft.Json.JsonConvert.SerializeObject(
                    new {
                        Success = true,
                        Data = await _swarmProxy.ConfigGetByModuleId(moduleId) 
                    }, Newtonsoft.Json.Formatting.Indented);

            }
            catch (DockerApiException e)
            {
                return Newtonsoft.Json.JsonConvert.SerializeObject(
                    new {
                        Success = false,
                        ErrorType = "DockerApiException",
                        Error = e.Message
                    }, Newtonsoft.Json.Formatting.Indented);
            }
            catch (Exception e)
            {
                return Newtonsoft.Json.JsonConvert.SerializeObject(
                    new {
                        Success = false,
                        ErrorType = "General",
                        Error = e.Message
                    }, Newtonsoft.Json.Formatting.Indented);
            }
        }
        
        [HttpDelete]
        [Route("Config/{configId}")]
        public async Task<ActionResult<string>> ConfigRemove([FromRoute] string configId)
        {
            try
            {
                await _swarmProxy.ConfigRemove(configId);
                return Newtonsoft.Json.JsonConvert.SerializeObject(
                    new {
                        Success = true,
                    }, Newtonsoft.Json.Formatting.Indented);

            }
            catch (DockerApiException e)
            {
                return Newtonsoft.Json.JsonConvert.SerializeObject(
                    new {
                        Success = false,
                        ErrorType = "DockerApiException",
                        Error = e.Message
                    }, Newtonsoft.Json.Formatting.Indented);
            }
            catch (Exception e)
            {
                return Newtonsoft.Json.JsonConvert.SerializeObject(
                    new {
                        Success = false,
                        ErrorType = "General",
                        Error = e.Message
                    }, Newtonsoft.Json.Formatting.Indented);
            }
        }
        
        
        [HttpPost]
        [Route("Volume/{batchId}/{moduleId}")]
        public async Task<ActionResult<string>> VolumeCreate(
            [FromRoute] string batchId, 
            [FromRoute] string moduleId, 
            [FromBody] XVolumeDescription volumeDescription)
        {
            try
            {
                return Newtonsoft.Json.JsonConvert.SerializeObject(
                    new {
                        Success = true,
                        Data = await _swarmProxy.VolumeCreate(volumeDescription, moduleId, batchId) 
                    }, Newtonsoft.Json.Formatting.Indented);

            }
            catch (DockerApiException e)
            {
                return Newtonsoft.Json.JsonConvert.SerializeObject(
                    new {
                        Success = false,
                        ErrorType = "DockerApiException",
                        Error = e.Message
                    }, Newtonsoft.Json.Formatting.Indented);
            }
            catch (Exception e)
            {
                return Newtonsoft.Json.JsonConvert.SerializeObject(
                    new {
                        Success = false,
                        ErrorType = "General",
                        Error = e.Message
                    }, Newtonsoft.Json.Formatting.Indented);
            }

        }  
        
        [HttpGet]
        [Route("Volume/{volumeId}")]
        public async Task<ActionResult<string>> VolumeGetById([FromRoute] string volumeId)
        {
            try
            {
                return Newtonsoft.Json.JsonConvert.SerializeObject(
                    new {
                        Success = true,
                        Data = await _swarmProxy.VolumeGetById(volumeId) 
                    }, Newtonsoft.Json.Formatting.Indented);

            }
            catch (DockerApiException e)
            {
                return Newtonsoft.Json.JsonConvert.SerializeObject(
                    new {
                        Success = false,
                        ErrorType = "DockerApiException",
                        Error = e.Message
                    }, Newtonsoft.Json.Formatting.Indented);
            }
            catch (Exception e)
            {
                return Newtonsoft.Json.JsonConvert.SerializeObject(
                    new {
                        Success = false,
                        ErrorType = "General",
                        Error = e.Message
                    }, Newtonsoft.Json.Formatting.Indented);
            }
        }
        
        [HttpGet]
        [Route("Volume/GetByModule/{moduleId}")]
        public async Task<ActionResult<string>> VolumeGetByModuleId([FromRoute] string moduleId)
        {
            try
            {
                return Newtonsoft.Json.JsonConvert.SerializeObject(
                    new {
                        Success = true,
                        Data = await _swarmProxy.VolumeGetByModuleId(moduleId) 
                    }, Newtonsoft.Json.Formatting.Indented);

            }
            catch (DockerApiException e)
            {
                return Newtonsoft.Json.JsonConvert.SerializeObject(
                    new {
                        Success = false,
                        ErrorType = "DockerApiException",
                        Error = e.Message
                    }, Newtonsoft.Json.Formatting.Indented);
            }
            catch (Exception e)
            {
                return Newtonsoft.Json.JsonConvert.SerializeObject(
                    new {
                        Success = false,
                        ErrorType = "General",
                        Error = e.Message
                    }, Newtonsoft.Json.Formatting.Indented);
            }
        }
        
        [HttpDelete]
        [Route("Volume/{volumeId}")]
        public async Task<ActionResult<string>> VolumeRemove([FromRoute] string volumeId)
        {
            try
            {
                await _swarmProxy.VolumeRemove(volumeId);
                return Newtonsoft.Json.JsonConvert.SerializeObject(
                    new {
                        Success = true,
                    }, Newtonsoft.Json.Formatting.Indented);

            }
            catch (DockerApiException e)
            {
                return Newtonsoft.Json.JsonConvert.SerializeObject(
                    new {
                        Success = false,
                        ErrorType = "DockerApiException",
                        Error = e.Message
                    }, Newtonsoft.Json.Formatting.Indented);
            }
            catch (Exception e)
            {
                return Newtonsoft.Json.JsonConvert.SerializeObject(
                    new {
                        Success = false,
                        ErrorType = "General",
                        Error = e.Message
                    }, Newtonsoft.Json.Formatting.Indented);
            }
        }
        
        
        [HttpPost]
        [Route("Service")]
        public async Task<ActionResult<string>> ServiceCreate([FromBody] XBalticModuleBuild moduleBuild)
        {
            try
            {
                return Newtonsoft.Json.JsonConvert.SerializeObject(
                    new {
                        Success = true,
                        Data = await _swarmProxy.ServiceCreate(moduleBuild) 
                    }, Newtonsoft.Json.Formatting.Indented);

            }
            catch (DockerApiException e)
            {
                return Newtonsoft.Json.JsonConvert.SerializeObject(
                    new {
                        Success = false,
                        ErrorType = "DockerApiException",
                        Error = e.Message
                    }, Newtonsoft.Json.Formatting.Indented);
            }
            catch (Exception e)
            {
                return Newtonsoft.Json.JsonConvert.SerializeObject(
                    new {
                        Success = false,
                        ErrorType = "General",
                        Error = e.Message
                    }, Newtonsoft.Json.Formatting.Indented);
            }

        }  
        
        [HttpGet]
        [Route("Service/{serviceId}")]
        public async Task<ActionResult<string>> ServiceGetById([FromRoute] string serviceId)
        {
            try
            {
                return Newtonsoft.Json.JsonConvert.SerializeObject(
                    new {
                        Success = true,
                        Data = await _swarmProxy.ServiceGetById(serviceId) 
                    }, Newtonsoft.Json.Formatting.Indented);
        
            }
            catch (DockerApiException e)
            {
                return Newtonsoft.Json.JsonConvert.SerializeObject(
                    new {
                        Success = false,
                        ErrorType = "DockerApiException",
                        Error = e.Message
                    }, Newtonsoft.Json.Formatting.Indented);
            }
            catch (Exception e)
            {
                return Newtonsoft.Json.JsonConvert.SerializeObject(
                    new {
                        Success = false,
                        ErrorType = "General",
                        Error = e.Message
                    }, Newtonsoft.Json.Formatting.Indented);
            }
        }
        
        [HttpGet]
        [Route("Service/GetByBatch/{batchId}")]
        public async Task<ActionResult<string>> ServiceGetByBatchId([FromRoute] string batchId)
        {
            try
            {
                return Newtonsoft.Json.JsonConvert.SerializeObject(
                    new {
                        Success = true,
                        Data = await _swarmProxy.ServiceGetByBatchId(batchId) 
                    }, Newtonsoft.Json.Formatting.Indented);
        
            }
            catch (DockerApiException e)
            {
                return Newtonsoft.Json.JsonConvert.SerializeObject(
                    new {
                        Success = false,
                        ErrorType = "DockerApiException",
                        Error = e.Message
                    }, Newtonsoft.Json.Formatting.Indented);
            }
            catch (Exception e)
            {
                return Newtonsoft.Json.JsonConvert.SerializeObject(
                    new {
                        Success = false,
                        ErrorType = "General",
                        Error = e.Message
                    }, Newtonsoft.Json.Formatting.Indented);
            }
        }
        
        [HttpDelete]
        [Route("Service/{serviceId}")]
        public async Task<ActionResult<string>> ServiceRemove([FromRoute] string serviceId)
        {
            try
            {
                await _swarmProxy.ServiceRemove(serviceId);
                return Newtonsoft.Json.JsonConvert.SerializeObject(
                    new {
                        Success = true,
                    }, Newtonsoft.Json.Formatting.Indented);
        
            }
            catch (DockerApiException e)
            {
                return Newtonsoft.Json.JsonConvert.SerializeObject(
                    new {
                        Success = false,
                        ErrorType = "DockerApiException",
                        Error = e.Message
                    }, Newtonsoft.Json.Formatting.Indented);
            }
            catch (Exception e)
            {
                return Newtonsoft.Json.JsonConvert.SerializeObject(
                    new {
                        Success = false,
                        ErrorType = "General",
                        Error = e.Message
                    }, Newtonsoft.Json.Formatting.Indented);
            }
        }


        [HttpGet]
        [Route("Container/GetByModule/{moduleId}")]
        public async Task<ActionResult<string>> ContainerGetByModuleId(string moduleId)
        {
            
            try
            {
                return Newtonsoft.Json.JsonConvert.SerializeObject(
                    new {
                        Success = true,
                        Data = await _portainerApi.ContainerGetByModuleId(moduleId)
                    }, Newtonsoft.Json.Formatting.Indented);
        
            }
            catch (DockerApiException e)
            {
                return Newtonsoft.Json.JsonConvert.SerializeObject(
                    new {
                        Success = false,
                        ErrorType = "DockerApiException",
                        Error = e.Message
                    }, Newtonsoft.Json.Formatting.Indented);
            }
            catch (Exception e)
            {
                return Newtonsoft.Json.JsonConvert.SerializeObject(
                    new {
                        Success = false,
                        ErrorType = "General",
                        Error = e.Message
                    }, Newtonsoft.Json.Formatting.Indented);
            }
        }
        
        
        [HttpPost]
        [Route("ClusterProxy/PrepareWorkspace")]
        public async Task<ActionResult<ClusterStatusResponse>> PrepareWorkspace(XWorkspace workspace)
        {
            return await _clusterProxy.PrepareWorkspace(workspace,  null);
        }
        
        [HttpGet]
        [Route("ClusterProxy/CheckWorkspaceStatus/{batchId}")]
        public async Task<ActionResult<ClusterStatusResponse>> CheckWorkspaceStatus([FromRoute]string batchId)
        {
            return await _clusterProxy.CheckWorkspaceStatus(new BatchId(){Id = batchId}, null);
        }
        
        [HttpDelete]
        [Route("ClusterProxy/PurgeWorkspace/{batchId}")]
        public async Task<ActionResult<ClusterStatusResponse>> PurgeWorkspace(string batchId)
        {
            return await _clusterProxy.PurgeWorkspace(new BatchId(){Id = batchId}, null);
        }
        
        [HttpPost]
        [Route("ClusterProxy/RunBalticModule")]
        public async Task<ActionResult<ClusterStatusResponse>> RunBalticModule([FromBody]XBalticModuleBuild build)
        {
            return await _clusterProxy.RunBalticModule(build, null);
        }
        
        [HttpGet]
        [Route("ClusterProxy/CheckBalticModuleStatus/{batchId}/{moduleId}")]
        public async Task<ActionResult<ClusterStatusResponse>> CheckBalticModuleStatus(string batchId, string moduleId)
        {
            return await _clusterProxy.CheckBalticModuleStatus(new Module(){BatchId = batchId, ModuleId = moduleId},null);
        }
        
        [HttpDelete]
        [Route("ClusterProxy/DisposeBalticModule/{batchId}/{moduleId}")]
        public async Task<ActionResult<ClusterStatusResponse>> DisposeBalticModule(string batchId, string moduleId)
        {
            return await _clusterProxy.DisposeBalticModule(new Module() {BatchId = batchId, ModuleId = moduleId}, null);
        }
        
        [HttpGet]
        [Route("ClusterProxy/GetClusterDescription")]
        public async Task<ActionResult<ClusterStatusResponse>> GetClusterDescription()
        {
            await Task.Run(() => System.Threading.Thread.Sleep(100));
            throw new NotImplementedException();
        }
    }
}

#endif