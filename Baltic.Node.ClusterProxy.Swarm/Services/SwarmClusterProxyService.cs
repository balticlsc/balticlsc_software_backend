using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Baltic.Types.Protos;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using System.Text.Json;
using AutoMapper.Internal;
using Baltic.Core.Extensions;
using Docker.DotNet;
using Docker.DotNet.Models;
using Newtonsoft.Json.Linq;
using Serilog;


namespace Baltic.Node.ClusterProxy.Swarm.Services
{
    public class SwarmClusterProxyService : Baltic.Types.Protos.ClusterProxy.ClusterProxyBase
    {
        private readonly ISwarmProxy _swarmProxy;
        private readonly IPortainerApi _portainerApi;
        
        public SwarmClusterProxyService(ISwarmProxy swarmProxy, IPortainerApi portainerApi)
        {
            _swarmProxy = swarmProxy;
            _portainerApi = portainerApi;
            
        }
        
        public override async Task<ClusterStatusResponse> PrepareWorkspace(XWorkspace workspace, ServerCallContext context) 
        { 
            Log.Information($"Attempt to prepare Workspace: {workspace.BatchId}");
            
            try
            {
                await _swarmProxy.NetworkCreate(workspace.BatchId);
            }
            catch (DockerApiException e) when ((string) JObject.Parse(e.ResponseBody)["message"] ==
                                               $"network with name {workspace.BatchId} already exists")
            {
                var errorMessage = $"Workspace with name {workspace.BatchId} already exists in a system"; 
                Log.Error(errorMessage);
                return new ClusterStatusResponse()
                {
                    Status = ClusterStatusResponse.Types.StatusCode.Error,
                    Message = errorMessage
                };
            }
            catch (DockerApiException e)
            {
                Log.Error($"Unknown DockerApi exception was thrown: \n {e.Message}");
                return new ClusterStatusResponse()
                {
                    Status = ClusterStatusResponse.Types.StatusCode.Error,
                    Message = e.Message
                };
            }
            catch (Exception e)
            {
                Log.Error($"Unknown exception was thrown: \n {e.Message}");
                return new ClusterStatusResponse()
                {
                    Status = ClusterStatusResponse.Types.StatusCode.Error,
                    Message = e.Message
                };
            }

            var message = $"Workspace: {workspace.BatchId} successfully created (Status: {ClusterStatusResponse.Types.StatusCode.Active.GetDescription()})";
            Log.Information(message);

            return new ClusterStatusResponse()
            {
                Status = ClusterStatusResponse.Types.StatusCode.Active,
                Message = message
            };
        }
        
        public override async Task<ClusterStatusResponse> CheckWorkspaceStatus(BatchId batchId, ServerCallContext context) 
        {
            Log.Information($"Attempt to check status of workspace: {batchId}");

            try
            {
                var network = await _swarmProxy.NetworkGetById(batchId.Id);
                if (network == null)
                {
                    return new ClusterStatusResponse()
                    {
                        Status = ClusterStatusResponse.Types.StatusCode.NotFound,
                        Message = $"Workspace with name {batchId.Id} not found"
                    };
                }

                return new ClusterStatusResponse()
                {
                    Status = ClusterStatusResponse.Types.StatusCode.Active,
                    Message = $"Workspace status: {ClusterStatusResponse.Types.StatusCode.Active.GetDescription()}"
                };
            }
            catch (DockerApiException e)
            {
                Log.Error($"Unknown DockerApi exception was thrown: \n {e.Message}");
                return new ClusterStatusResponse()
                {
                    Status = ClusterStatusResponse.Types.StatusCode.Error,
                    Message = e.Message
                };
            }
            catch (Exception e)
            {
                Log.Error($"Unknown exception was thrown: \n {e.Message}");
                return new ClusterStatusResponse()
                {
                    Status = ClusterStatusResponse.Types.StatusCode.Error,
                    Message = e.Message
                };
            }
        }
        
        public override async Task<ClusterStatusResponse> PurgeWorkspace(BatchId batchId, ServerCallContext context) 
        {
            Log.Information($"Attempt to purge Workspace network with name: {batchId}");
            
            var services = await _swarmProxy.ServiceGetByBatchId(batchId.Id);
            
            List<Task> removeTasks = new List<Task>();
            foreach (var service in services)
            {
                removeTasks.Add(
                    DisposeBalticModule(new Module()
                    {
                        BatchId = batchId.Id,
                        ModuleId = service.Spec.Name
                    }, context));
            }
            
            try
            {
                await Task.WhenAll(removeTasks);
            }
            catch (Exception e)
            {

                var sb = new StringBuilder();
                foreach (var exception in removeTasks.Where(t => t.Exception != null)
                    .Select(t => t.Exception))
                {
                    sb.AppendLine($"{exception.GetType()}: {exception.Message}; Exception stack: \n {exception.StackTrace}");
                }

                string msgError = sb.ToString();
                Log.Error(msgError);
                
                return new ClusterStatusResponse()
                {
                    Status = ClusterStatusResponse.Types.StatusCode.Error,
                    Message = msgError
                };
            }
            
            await _swarmProxy.NetworkRemove(batchId.Id);

            var msg = $"Workspace: {batchId} successfully purged";
            Log.Information(msg);
            return new ClusterStatusResponse()
                {
                    Status = ClusterStatusResponse.Types.StatusCode.NotFound,
                    Message = msg
                };
        }
        
        public override async Task<ClusterStatusResponse> RunBalticModule(XBalticModuleBuild balticModuleBuild, ServerCallContext context)
        {
            var info = System.Text.Json.JsonSerializer.Serialize(balticModuleBuild, new JsonSerializerOptions{WriteIndented = true});
            
            Log.Information($"Attempt to start Baltic Module: \n {info}");

            try
            {
                await _swarmProxy.ServiceCreate(balticModuleBuild);

                var clusterResponse = new ClusterStatusResponse()
                {
                    Status = ClusterStatusResponse.Types.StatusCode.Pending,
                    Message = "Service was successfully created."
                };

                return clusterResponse;
            }
            catch (DockerApiException e) when ((string) JObject.Parse(e.ResponseBody)["message"] ==
                                               $"rpc error: code = AlreadyExists desc = name conflicts with an existing object: service {balticModuleBuild.ModuleId} already exists")
            {
                var errorMessage = $"Module with name {balticModuleBuild.ModuleId} already exists in a node";
                Log.Error(errorMessage);
                return new ClusterStatusResponse()
                {
                    Status = ClusterStatusResponse.Types.StatusCode.Error,
                    Message = errorMessage
                };
            }
            catch (DockerApiException e)
            {
                Log.Error($"Unknown DockerApi exception was thrown: \n {e.Message}");
                return new ClusterStatusResponse()
                {
                    Status = ClusterStatusResponse.Types.StatusCode.Error,
                    Message = e.Message
                };
            }
            catch (Exception e)
            {
                Log.Error($"Unknown exception was thrown: \n {e.Message}");
                return new ClusterStatusResponse()
                {
                    Status = ClusterStatusResponse.Types.StatusCode.Error,
                    Message = e.Message
                };
            }
        }
        
        public override async Task<ClusterStatusResponse> CheckBalticModuleStatus(Module module, ServerCallContext context) 
        {
            // TODO: The current solution does not take into account the container that crushes after the start.
            Log.Information($"Attempt to check Module status (Module Id: {module.ModuleId})");
            
            try
            {
                var moduleContainers = await _portainerApi.ContainerGetByModuleId(module.ModuleId);
                if (moduleContainers.Any(c => c.State == "running"))
                {
                    var clusterResponse = new ClusterStatusResponse()
                    {
                        Status = ClusterStatusResponse.Types.StatusCode.Active,
                        Message = "Module container is Running"
                    };
                    return clusterResponse;
                }
                else
                {
                    var clusterResponse = new ClusterStatusResponse()
                    {
                        Status = ClusterStatusResponse.Types.StatusCode.Pending,
                        Message = $"There is no running container for this module (Module Id: {module.ModuleId})"
                    };
                    return clusterResponse;
                }
            }
            catch (DockerApiException e)
            {
                Log.Error($"Unknown DockerApi exception was thrown: \n {e.Message}");
                return new ClusterStatusResponse()
                {
                    Status = ClusterStatusResponse.Types.StatusCode.Error,
                    Message = e.Message
                };
            }
            catch (Exception e)
            {
                Log.Error($"Unknown exception was thrown: \n {e.Message}");
                return new ClusterStatusResponse()
                {
                    Status = ClusterStatusResponse.Types.StatusCode.Error,
                    Message = e.Message
                };
            }
        }
        
        public override async Task<ClusterStatusResponse> DisposeBalticModule(Module module, ServerCallContext context) 
        {
            var info = Newtonsoft.Json.JsonConvert.SerializeObject(module, Newtonsoft.Json.Formatting.Indented);
            
            Log.Information($"Attempt to dispose Baltic Module: \n {info}");

            await _swarmProxy.ServiceRemove(module.ModuleId);

            var getConfigs = _swarmProxy.ConfigGetByModuleId(module.ModuleId);
            var getVolumes = _swarmProxy.VolumeGetByModuleId(module.ModuleId);
            
            // ClusterStatusResponse moduleStatus;
            IEnumerable<ContainerListResponse> moduleContainers;
            do
            {
                moduleContainers = await _portainerApi.ContainerGetByModuleId(module.ModuleId);
                System.Threading.Thread.Sleep(1000);
            } while (moduleContainers != null && moduleContainers.Any());
            
            await Task.WhenAll(getConfigs, getVolumes);
            
            List<Task> removeTasks = new List<Task>();
            var configs = await getConfigs;
            if (configs != null)
            {
                foreach (var config in configs)
                {
                    removeTasks.Add(_swarmProxy.ConfigRemove(config.ID));
                }
            }
            
            var volumes = await _swarmProxy.VolumeGetByModuleId(module.ModuleId);
            if (volumes != null)
            {
                foreach (var volume in volumes)
                {
                    // wait until volume will not be in use
                    // TODO: Create proper method for that
                    VolumesListResponse moduleVolumes;
                    do
                    {
                        var response = await _portainerApi.CustomCall($"api/endpoints/{_portainerApi.GetPortainerEndpointsId()}/docker/volumes?filters=%7B%22dangling%22%3A%5B%22true%22%5D%7D");
                        var dockerJsonSerializer = new Docker.DotNet.JsonSerializer();
                        moduleVolumes = dockerJsonSerializer.DeserializeObject<VolumesListResponse>(response);
                        System.Threading.Thread.Sleep(1000);
                    } while (moduleVolumes.Volumes.All(v => v.Name != volume.Name));
                    
                    removeTasks.Add(_swarmProxy.VolumeRemove(volume.Name));
                }
            }

            try
            {
                await Task.WhenAll(removeTasks);
            }
            catch (Exception e)
            {

                var sb = new StringBuilder();
                foreach (var exception in removeTasks.Where(t => t.Exception != null)
                .Select(t => t.Exception))
                {
                    sb.AppendLine($"{exception.GetType()}: {exception.Message}; Exception stack: \n {exception.StackTrace}");
                }

                string msgError = sb.ToString();
                Log.Error(msgError);
                
                return new ClusterStatusResponse()
                {
                    Status = ClusterStatusResponse.Types.StatusCode.Error,
                    Message = msgError
                };
            }
            
            string msgSuccess = $"Module {module.BatchId} and its components successfully removed.";
            Log.Information(msgSuccess);
            
            return new ClusterStatusResponse()
            {
                Status = ClusterStatusResponse.Types.StatusCode.NotFound,
                Message = msgSuccess
            };
        }
        
        public override async Task<XClusterDescription> GetClusterDescription(Empty empty, ServerCallContext context) 
        {
            Log.Information("CheckBalticModuleStatus not yet implemented");
            await Task.Run(() => System.Threading.Thread.Sleep(250));
            var clusterResponse = new ClusterStatusResponse()
            {
                Status = ClusterStatusResponse.Types.StatusCode.Active,
                Message = "Not yet implemented"
            };
            return new XClusterDescription();
        }
        
    }
}