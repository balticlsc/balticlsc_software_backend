using System;
using Baltic.Consul.Exceptions;
using Baltic.Core.Utils;
using Consul;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Serilog;

namespace Baltic.Consul
{
    public static class ConsulExtensions
    {
        private static readonly ConsulOptions ConsulOptions = new ConsulOptions();
        private static ConsulClient _consulClient = null;
        public static void AddConsul(this IServiceCollection services, IConfiguration configuration)
        {
            configuration.GetSection("ConsulSettings").Bind(ConsulOptions);
            ConsulOptions.Id = $"{DateTime.UtcNow.GetHashCode():X}";
            
            if (ConsulOptions.IsValid())
            {
                try
                {
                    _consulClient = new ConsulClient(consulConfig =>
                        {
                            consulConfig.Address = ConsulOptions.ConsulAddress;
                        });
                    services.AddSingleton<IConsulClient, ConsulClient>(p => _consulClient);                    
                    ConsulOptions.Enabled = true;
                }
                catch (Exception e)
                {
                    throw new ConsulServiceException(e.Message, ConsulOptions.Host);
                }
            }
            else
            {
                throw new ConsulServiceException("Wrong Consul Service address");
            }
        }

        private static void OnExit(object sender, EventArgs eventArgs)
        {
            if (_consulClient != null)
            {
                _consulClient.Agent.ServiceDeregister(ConsulOptions.Id).Wait();                
            }
        }        
        
        public static void UseConsul(this IApplicationBuilder app)
        {
            if (ConsulOptions.Enabled && _consulClient != null)
            {
                AppDomain.CurrentDomain.ProcessExit += OnExit;

                var checkPoint =  $"{(ConsulOptions.UseSSL ? "https" : "http")}://{SystemInfo.GetIpAddress()}:{ConsulOptions.ServicePort}/healthz/live?id={ConsulOptions.Id}";
                
                if (SystemInfo.Windows)
                {
                    checkPoint =  $"{(ConsulOptions.UseSSL ? "https" : "http")}://host.docker.internal:{ConsulOptions.ServicePort}/healthz/live?id={ConsulOptions.Id}";                    
                }

                Log.Information("Register consul check point: {checkPoint}", checkPoint);

                var registration = new AgentServiceRegistration()
                {
                    ID = ConsulOptions.Id,
                    Name = ConsulOptions.ServiceName,
                    Address = $"{SystemInfo.GetIpAddress()}",
                    Port = ConsulOptions.ServicePort
                };

                if (ConsulOptions.CheckService)
                {
                    registration.Checks = new AgentServiceCheck[]
                    {
                        new AgentCheckRegistration()
                        {
                            HTTP = checkPoint,
                            Timeout = TimeSpan.FromSeconds(ConsulOptions.ServiceHealthCheckTimeOut),
                            Interval = TimeSpan.FromSeconds(ConsulOptions.ServiceHealthCheckInterval),
                            DeregisterCriticalServiceAfter = TimeSpan.FromSeconds(ConsulOptions.DeregisterCriticalServiceAfter),
                            TLSSkipVerify = ConsulOptions.UseSSL && !ConsulOptions.TLSSkipVerify,
                            Notes = DateTime.UtcNow.ToString("O"),
                            ServiceID = ConsulOptions.Id,
                            Name = ConsulOptions.Id
                        }
                    };
                }
                
                _consulClient.Agent.ServiceDeregister(ConsulOptions.Id);

                _consulClient.Agent.ServiceRegister(registration);

                //_consulClient.Agent.DisableNodeMaintenance();
                //_consulClient.Agent.EnableServiceMaintenance(ConsulOptions.Id, "Awaria węzła - coś zdechło");
                
                if (ConsulOptions.CheckService)
                {
                    app.UseEndpoints(endpoints =>
                    {
                        endpoints.MapHealthChecksControllers(ConsulOptions, _consulClient);
                    });
                }
            }
        }
    }
}