using System;
using System.Collections.Generic;
using System.Data;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Docker.DotNet.Models;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Serilog;
using JsonSerializer = Newtonsoft.Json.JsonSerializer;

namespace Baltic.Node.ClusterProxy.Swarm.Services
{
    public class PortainerApiWrapper : IPortainerApi
    {
        private readonly HttpClient _portainerClient;
        private readonly string _portainerPassword;
        private readonly string _portainerUsername;
        private readonly string _portainerEndpointsId;
        private static string Jwt { get; set; }
        
        public PortainerApiWrapper(HttpClient httpClient, IConfiguration configuration)
        {
            _portainerEndpointsId = configuration["portainerEndpointsId"] ?? "1";
            _portainerClient = httpClient;
            try
            {
                _portainerUsername = configuration["portainerUsername"] ?? throw new ArgumentNullException();
                _portainerPassword = configuration["portainerPassword"] ?? throw new ArgumentNullException();
            }
            catch (ArgumentNullException e)
            {
                var msg = "There is no portainer credential in configuration";
                Log.Error(msg);
                throw new ArgumentNullException(msg);
            }

            if (string.IsNullOrEmpty(Jwt) || IsJwtExpired())
            {
                GetNewJwt();
            }
            else
            {
                _portainerClient.DefaultRequestHeaders.Add("Authorization", "Bearer " + Jwt);
            }
        }

        private static bool IsJwtExpired()
        {
            var jwtHandler = new JwtSecurityTokenHandler();
            var jwtToken = jwtHandler.ReadToken(Jwt);
            var expDate = jwtToken.ValidTo;

            return expDate < DateTime.UtcNow.AddMinutes(1);
        }
        
        private void GetNewJwt()
        {
            var loginData = Newtonsoft.Json.JsonConvert.SerializeObject(new
            {
                Username = _portainerUsername,
                Password = _portainerPassword
            });
            Log.Debug($"Login to portainer with following credential: \n \tUser: {_portainerUsername} \n \tPass: {_portainerPassword}");
            var content = new StringContent(loginData, Encoding.UTF8, "application/json");
            HttpResponseMessage response;
            try
            {
                response = _portainerClient.PostAsync("api/auth", content).Result;
            }
            catch (Exception e)
            {
                throw new Exception("Portainer login failed.");
            }
            
            if (response != null)
            {
                var jsonString = response.Content.ReadAsStringAsync().Result;
                try
                {
                    Jwt = (string) JObject.Parse(jsonString)["jwt"];
                    Log.Debug($"Following token will be used: \n{Jwt}");
                    _portainerClient.DefaultRequestHeaders.Add("Authorization", "Bearer " + Jwt);
                }
                catch (Exception e)
                {
                    throw new Exception("Portainer login failed. Response does not contain jwt.");
                }
            }
            else
            {
                throw new Exception("Portainer login failed. Empty response from Portainer.");
            }
            
        }
        public async Task<IEnumerable<ContainerListResponse>> ContainerGetByModuleId(string moduleId)
        {
            Log.Information($"Attempt to find Container related to Module: {moduleId}");
            
            try
            {
                //_portainerClient.DefaultRequestHeaders.TryAddWithoutValidation("Authorization", "Bearer " + Jwt);
                var response = await _portainerClient.GetStringAsync($"api/endpoints/{_portainerEndpointsId}/docker/containers/json");
                var dockerJsonSerializer = new Docker.DotNet.JsonSerializer();
                var containerListResponses = dockerJsonSerializer.DeserializeObject<ContainerListResponse[]>(response).ToList();

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
            catch (HttpRequestException e)
            {
                Log.Error($"Unknown exception was thrown: \n {e.Message}");
                throw;
            }
        }
        
        public async Task<string> CustomCall(string uriPath)
        {
            var response = await _portainerClient.GetStringAsync(uriPath);
            return response;
        }

        public string GetPortainerEndpointsId() => _portainerEndpointsId;
    }
}