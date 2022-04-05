using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Docker.DotNet;
using Docker.DotNet.Models;
using Newtonsoft.Json.Linq;
using Serilog;

namespace Baltic.Node.ClusterProxy.Swarm.Services
{
    public partial class DockerApiWrapper
    {
        public async Task<IEnumerable<ContainerListResponse>> ContainerGetByModuleId(string moduleId)
        {
            // TODO: Do not work in cluster!!! Works only for one machine.
            Log.Information($"Attempt to find Container related to Module: {moduleId}");

            var containersListParams = new ContainersListParameters() {All = true};
            try
            {
                var containerListResponses = await _dockerClient.Containers.ListContainersAsync(containersListParams);
                var containersFound = containerListResponses.Where(
                    c => (c.Labels.ContainsKey("com.docker.swarm.service.name") && c.Labels["com.docker.swarm.service.name"] == moduleId)).ToList();

                if (containersFound.Any())
                {
                    Log.Information($"Docker client found Container related to Module: {moduleId}");
                    return containersFound;
                }

                Log.Information($"Docker client did not found any Container related to Module: {moduleId}");
                return containersFound;
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