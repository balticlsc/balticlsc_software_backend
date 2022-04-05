using System;
using Baltic.Node.ClusterProxy.Swarm.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.AspNetCore.Routing;
using Serilog;

namespace Baltic.Node.ClusterProxy.Swarm
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }
        
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddGrpc();
#if DEBUG
            services.AddControllers().AddNewtonsoftJson();;
#endif
            services.Configure<ConsoleLifetimeOptions>(opts =>
            {
                opts.SuppressStatusMessages = true;
            });

            services.AddScoped<ISwarmProxy,DockerApiWrapper>();
            string portainerUrl = Configuration["portainerUrl"] ?? "http://host.docker.internal:9000";
            
            services.AddHttpClient<IPortainerApi,PortainerApiWrapper>(client =>
            {
                client.BaseAddress = new Uri(portainerUrl);
            });
        }
        
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            
            app.UseRouting();
            
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapGrpcService<SwarmClusterProxyService>();

                endpoints.MapGet("/", async context =>
                {
                    await context.Response.WriteAsync("Communication with Baltic.Node endpoints must be made through a proper software. To learn about the software and project, visit: https://www.balticlsc.eu");
                });

#if DEBUG
                if (env.IsDevelopment())
                {
                    endpoints.MapControllerRoute("default","{controller=Debug}/{action}");
                }
#endif
            });
        }
    }
}