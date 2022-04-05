using Baltic.Core.Utils;
using Consul;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Newtonsoft.Json;

namespace Baltic.Consul
{
    public static class HealthChecks
    {
        public static IEndpointRouteBuilder MapHealthChecksControllers(
            this IEndpointRouteBuilder endpoints, ConsulOptions consulOptions, ConsulClient consulClient)
        {
            // live will not execute any test and will just return 200 if its up and running
            endpoints.MapHealthChecks("/healthz/live", new HealthCheckOptions()
            {
                Predicate = _ => false,
                ResponseWriter = async (context, report) =>
                {
                    string id = context.Request.Query["Id"];
                    if (id == consulOptions.Id || id == null)
                    {
                        AgentCheck agent = new AgentCheck()
                        {
                            Status = HealthStatus.Warning,
                            CheckID = consulOptions.Id,
                            Name = consulOptions.Id,
                            ServiceName = consulOptions.ServiceName,
                            Notes = $"{SystemInfo.UpTime}",
                            Output = $"{SystemInfo.UpTime}",
                            ServiceID = consulOptions.Id,
                            Node = consulClient.Agent.GetNodeName().Result
                        };
                        await context.Response.WriteAsync(JsonConvert.SerializeObject(agent));
                    }
                    else
                    {
                        context.Response.StatusCode = 403;
                    }
                }
            });

            return endpoints;
        }
    }
}