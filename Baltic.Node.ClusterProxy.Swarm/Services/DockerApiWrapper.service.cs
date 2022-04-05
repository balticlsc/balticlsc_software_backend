using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Baltic.Core.Extensions;
using Baltic.Types.Protos;
using Docker.DotNet;
using Docker.DotNet.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using Serilog;

namespace Baltic.Node.ClusterProxy.Swarm.Services
{
    public partial class DockerApiWrapper
    {
        public async Task<string> ServiceCreate([FromBody] XBalticModuleBuild balticModuleBuild)
        {
            Log.Information($"Attempt to start computation module: {balticModuleBuild.ModuleId} for batch: ${balticModuleBuild.BatchId}");
            var labels = new Dictionary<string, string>
            {
                {DockerStackLabel, balticModuleBuild.BatchId},
                {BalticModuleLabel, balticModuleBuild.ModuleId}
            };

            // Fetching network assigned to batch, throws error if not found
            var batchNetwork = await FetchNetwork(balticModuleBuild.BatchId);

            // Translating network to Docker Swarm Rest API
            var networksRestApi = new List<NetworkAttachmentConfig>();
            networksRestApi.Add(new NetworkAttachmentConfig()
                {
                    Aliases = new List<string>()
                    {
                        balticModuleBuild.ModuleId,
                        $"{balticModuleBuild.ModuleId}.{_projectPrefix}-{balticModuleBuild.BatchId}"
                    },
                    Target = batchNetwork.ID
                });
            
            // TODO: isModuleManager as method parameter
            var isModuleManager = true;
            if (isModuleManager)
            {
                // TODO: Unflat node network!!!
                // Fetching system network, throws error if not found
                NetworkResponse batchManagerNetwork = await FetchNetwork("balticlsc-node"); // node networks were flatted temporary
                // NetworkResponse batchManagerNetwork = await FetchNetwork($"{_projectPrefix}-{_batchManagerNetworkName}"); // proper network name 
            
                networksRestApi.Add(new NetworkAttachmentConfig()
                {
                    Aliases = new List<string>()
                    {
                        balticModuleBuild.ModuleId,
                        $"{balticModuleBuild.ModuleId}.{_projectPrefix}-{balticModuleBuild.BatchId}"
                    },
                    Target = batchManagerNetwork.ID
                });
            }
            
            // Translating ports to Rest API
            var portMappingRestApi = new List<PortConfig>();
            switch (balticModuleBuild.Scope)
            {
                case XBalticModuleBuild.Types.NetworkScope.Workspace:
                    // do nothing not needed in DockerSwarm
                    // for kubernetes ports need to be open for service
                    break;
                
                case XBalticModuleBuild.Types.NetworkScope.Cluster:
                    if (balticModuleBuild.PortMappings != null)
                    {
                        foreach (var portMapping in balticModuleBuild.PortMappings)
                        {
                            portMappingRestApi.Add( 
                                new PortConfig()
                                {
                                    Protocol = portMapping.Protocol.GetDescription(),
                                    TargetPort = portMapping.ContainerPort,
                                    PublishedPort = portMapping.PublishedPort,
                                    PublishMode = "ingress"
                                });
                        }
                    }
                    break;
                
                case XBalticModuleBuild.Types.NetworkScope.Public:
                    throw new NotImplementedException("Setting public host is not yet supported.");
            }

            // Translating configs to Docker Swarm Rest API
            var configsRestApi = new List<SwarmConfigReference>();
            if (balticModuleBuild.ConfigFiles !=  null)
            {
                
                foreach (var configFile in balticModuleBuild.ConfigFiles)
                {
                    // creating docker configs needed by service    
                    var configId = await ConfigCreate(configFile, balticModuleBuild.ModuleId, balticModuleBuild.BatchId);
                    
                    configsRestApi.Add(
                        new SwarmConfigReference()
                        {
                            File = new ConfigReferenceFileTarget()
                            {
                                Name = configFile.MountPath,
                                UID = "0",
                                GID = "0",
                                Mode = 0444
                            },
                            ConfigID = configId,
                            ConfigName = DockerSwarmHelper.CreateConfigName(configFile, balticModuleBuild.ModuleId, balticModuleBuild.BatchId)
                        });
                }
            }
            
            // Translating volumes to Docker Swarm Rest API
            var volumesRestApi = new List<Mount>();
            if (balticModuleBuild.Volumes != null)
            {
                foreach (var volumeDescription in balticModuleBuild.Volumes)
                {
                    await VolumeCreate(volumeDescription, balticModuleBuild.ModuleId, balticModuleBuild.BatchId);
                    volumesRestApi.Add(
                        new Mount()
                        {
                            Source = DockerSwarmHelper.CreateVolumeName(volumeDescription, balticModuleBuild.ModuleId, balticModuleBuild.BatchId),
                            Type = "volume",
                            Target = volumeDescription.MountPath
                        });
                }
            }
            
            // Translating service to Docker Swarm Rest API
            var envs = new List<string>();
            if (balticModuleBuild.EnvironmentVariables != null && balticModuleBuild.EnvironmentVariables.Count != 0)
            {
                foreach (var xEnvironmentVariable in balticModuleBuild.EnvironmentVariables)
                {
                    envs.Add(xEnvironmentVariable.ToEnvString());
                }
            }

            var containerSpec = new ContainerSpec()
            {
                Image = balticModuleBuild.Image,
                Labels = new Dictionary<string, string>()
                {
                    {DockerStackLabel, balticModuleBuild.BatchId},
                },
                Args = balticModuleBuild.CommandArguments?.ToList(),
                Hostname = balticModuleBuild.ModuleId,
                Env = envs,
                TTY = false,
                OpenStdin = true,
                ReadOnly = false,
                Mounts = volumesRestApi,
                Configs = configsRestApi
            };

            // Docker Swarm does not support setting storage limit
            var serviceCreateParameters = new ServiceCreateParameters()
            {
                Service = new ServiceSpec()
                {
                    Name = balticModuleBuild.ModuleId,
                    Labels = labels,
                    TaskTemplate = new TaskSpec()
                    {
                        ContainerSpec = containerSpec,
                        Resources = new ResourceRequirements()
                        {
                            Limits = new SwarmResources()
                            {
                                MemoryBytes = 1024*1024*balticModuleBuild.Resources.Memory, // B to MB
                                NanoCPUs = 1000000*balticModuleBuild.Resources.Cpus // mili to nano
                            },
                            Reservations = new SwarmResources()
                            {
                                MemoryBytes = 1024*1024*balticModuleBuild.Resources.Memory, // B to MB
                                NanoCPUs = 1000000*balticModuleBuild.Resources.Cpus // mili2nano
                            }
                        },
                        Runtime = "container",
                        Networks = networksRestApi,
                        LogDriver = new SwarmDriver()
                    },
                    Mode = new ServiceMode()
                    {
                        Replicated = new ReplicatedService()
                        {
                            Replicas = 1
                        }
                    },
                    Networks = networksRestApi,
                    EndpointSpec = new EndpointSpec()
                    {
                        Mode = "vip",
                        Ports = portMappingRestApi
                    }
                }
            };
            
            try
            {
                var createResponse = await _dockerClient.Swarm.CreateServiceAsync(serviceCreateParameters);
                Log.Information($"Docker client created Service with name: {balticModuleBuild.ModuleId}.");
                return createResponse.ID;
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
        
        public async Task<SwarmService> ServiceGetById(string serviceId)
        {
            Log.Information($"Attempt to find Computation Module with name: {serviceId}");
            try
            {
                var services = await _dockerClient.Swarm.ListServicesAsync();
                var foundService = services.SingleOrDefault(s => s.Spec.Name == serviceId);
                Log.Information($"Docker client found Computation Module with name: {serviceId}");
                return foundService;
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
        
        public async Task<IEnumerable<SwarmService>> ServiceGetByBatchId(string batchId)
        {
            Log.Information($"Attempt to find Modules related to Batch: {batchId}");
            try
            {
                var services = await _dockerClient.Swarm.ListServicesAsync();
                var serviceGetByBatchId = services.ToList();
                serviceGetByBatchId = serviceGetByBatchId.Where(s => s.Spec.Labels.ContainsKey(DockerStackLabel) && s.Spec.Labels[DockerStackLabel] == batchId).ToList();
                if (serviceGetByBatchId.Any())
                {
                    Log.Information($"Docker client found Modules related to Batch: {batchId}");
                    return serviceGetByBatchId;
                }
                Log.Information($"Docker client did not found any Modules related to Batch: {batchId}");
                return serviceGetByBatchId;
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
        
        public async Task ServiceRemove(string serviceId)
        {
            Log.Information($"Attempt to dispose Service with name: {serviceId}");
            try
            {
                var service = await ServiceGetById(serviceId);
                if (service == null)
                {
                    Log.Information($"Service with name: {serviceId} not found (Already deleted or wrong name.)");    
                }
                else
                {
                    await _dockerClient.Swarm.RemoveServiceAsync(service.ID);
                    Log.Information($"Docker client dispose Service with name: {serviceId} (image: {service.Spec.TaskTemplate.ContainerSpec.Image})");
                }
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