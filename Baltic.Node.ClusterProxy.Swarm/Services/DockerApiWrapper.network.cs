using System;
using System.Linq;
using System.Text;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Threading;
using System.Threading.Tasks;
using Baltic.Core.Extensions;
using Baltic.Types.Protos;
using Docker.DotNet;
using Docker.DotNet.Models;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json.Linq;
using Serilog;

namespace Baltic.Node.ClusterProxy.Swarm.Services
{
    public partial class DockerApiWrapper
    {
        public async Task<string> NetworkCreate(string networkId)
        {
            Log.Information($"Attempt to create network with name: {networkId}");
            var labels = new Dictionary<string, string> {{DockerStackLabel, networkId}};
            
            NetworksCreateParameters networkParameters = new NetworksCreateParameters()
            {
                Name = networkId,
                Driver = "overlay",
                Labels = labels,
            };
            
            try
            {
                var networkResponse = await _dockerClient.Networks.CreateNetworkAsync(networkParameters);
                Log.Information($"Docker client created network with Name: {networkId} and ID: {networkResponse.ID}");
                return networkResponse.ID;
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
        
        // NetworkGetById -> return one or null if not found
        public async Task<NetworkResponse> NetworkGetById(string networkId)
        {
            Log.Information($"Attempt to find network with name: {networkId}");
            try
            {
                var networkResponse = await _dockerClient.Networks.ListNetworksAsync();
                var foundNetwork = networkResponse.SingleOrDefault((n) => (n.Name == networkId && n.Driver == "overlay") );
                Log.Information(foundNetwork == null
                    ? $"Docker client did not find network with name: {networkId}"
                    : $"Docker client found network with name: {networkId} and ID: {foundNetwork?.ID}");
                return foundNetwork;

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
        
        public async Task NetworkRemove(string networkId)
        {
            Log.Information($"Attempt to remove network with name: {networkId}");
            try
            {
                await _dockerClient.Networks.DeleteNetworkAsync(networkId);
                Log.Information($"Docker client starts removing network with Name: {networkId} from a system");
            }
            catch (Exception e)
            {
                Log.Error(e.InnerException?.Message);
                throw;
            }
        }

        // FetchNetwork -> return one or throws error if not found
        private async Task<NetworkResponse> FetchNetwork(string networkId)
        {
            int k = 1;
            NetworkResponse network;
            do
            {
                if (k != 1) Thread.Sleep(1000);
                Log.Information($"Checking if network {networkId} exists ... [{k++}]");
                network = await NetworkGetById(networkId);
                if (k > 120) throw new Exception($"Network {networkId} is not ready.");
            } while (network == null);

            return network;
        }
    }
}