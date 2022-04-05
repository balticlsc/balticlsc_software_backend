using System;
using Baltic.Consul.Exceptions;
using Baltic.Core.Utils;
using Baltic.Web.Module;
using Consul;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Serilog;

namespace Baltic.Consul
{
    public class ConsulModule : IModule
    {
        private readonly ConsulOptions _consulOptions = new ConsulOptions();
        private ConsulClient _consulClient;
        
        public void AddModule(IServiceCollection services, IConfiguration configuration, IWebHostEnvironment environment = null)
        {
            configuration.GetSection("ConsulSettings").Bind(_consulOptions);
            _consulOptions.Id = $"{DateTime.UtcNow.GetHashCode():X}";

            if (_consulOptions.IsValid())
            {
                try
                {
                    _consulClient = new ConsulClient(consulConfig =>
                    {
                        consulConfig.Address = _consulOptions.ConsulAddress;
                    });
                    services.AddSingleton<IConsulClient, ConsulClient>(p => _consulClient);
                    _consulOptions.Enabled = true;
                }
                catch (Exception e)
                {
                    throw new ConsulServiceException(e.Message, _consulOptions.Host);
                }
            }
            else
            {
                throw new ConsulServiceException("Wrong Consul Service address");
            }
        }

        private void OnExit(object sender, EventArgs eventArgs)
        {
            if (_consulClient != null)
            {
                _consulClient.Agent.ServiceDeregister(_consulOptions.Id).Wait();
            }
        }
        public void UseModule(IApplicationBuilder app)
        {
            if (_consulOptions.Enabled && _consulClient != null)
            {
                AppDomain.CurrentDomain.ProcessExit += OnExit;

                var checkPoint =
                    $"{(_consulOptions.UseSSL ? "https" : "http")}://{Core.Utils.SystemInfo.GetIpAddress()}:{_consulOptions.ServicePort}/healthz/live?id={_consulOptions.Id}";

                if (SystemInfo.Windows)
                {
                    checkPoint =
                        $"{(_consulOptions.UseSSL ? "https" : "http")}://host.docker.internal:{_consulOptions.ServicePort}/healthz/live?id={_consulOptions.Id}";                    
                }
                
                Log.Information("Register consul check point: {checkPoint}", checkPoint);                
                
                var registration = new AgentServiceRegistration()
                {
                    ID = _consulOptions.Id,
                    Name = _consulOptions.ServiceName,
                    Address = SystemInfo.GetIpAddress(),
                    Port = _consulOptions.ServicePort
                };
                
                if (_consulOptions.CheckService)
                {
                    registration.Checks = new AgentServiceCheck[]
                    {
                        new AgentCheckRegistration()
                        {
                            HTTP = checkPoint,
                            Timeout = TimeSpan.FromSeconds(_consulOptions.ServiceHealthCheckTimeOut),
                            Interval = TimeSpan.FromSeconds(_consulOptions.ServiceHealthCheckInterval),
                            DeregisterCriticalServiceAfter =
                                TimeSpan.FromSeconds(_consulOptions.DeregisterCriticalServiceAfter),
                            TLSSkipVerify = false,
                            Notes = DateTime.UtcNow.ToString("O"),
                            ServiceID = _consulOptions.Id,
                            Name = _consulOptions.Id,
                            Status = HealthStatus.Passing
                        }
                    };
                }

                _consulClient.Agent.ServiceDeregister(_consulOptions.Id);

                _consulClient.Agent.ServiceRegister(registration);

                _consulClient.Agent.DisableNodeMaintenance();
                //_consulClient.Agent.EnableServiceMaintenance(ConsulOptions.Id, "Awaria węzła - coś zdechło");

                if (_consulOptions.CheckService)
                {
                    app.UseEndpoints(endpoints =>
                    {
                        endpoints.MapHealthChecksControllers(_consulOptions, _consulClient);
                    });
                }
            }
        }
    }
}