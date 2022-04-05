using Baltic.CommonServices;
using Baltic.Engine.JobBroker;
using Baltic.Node.BatchManager.Controllers;
using Baltic.Node.BatchManager.Proxies;
using Baltic.Node.Engine.BatchManager;
using Baltic.Node.Engine.DataAccess;
using Baltic.Node.Engine.ServerAccess;
using Baltic.Types.Entities;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Baltic.Node
{
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton(new NodeManager());
            services.Configure<ConsoleLifetimeOptions>(opts =>
            {
                opts.SuppressStatusMessages = true;
            });
            
            //services.AddModule<BatchManagerModule>(Configuration);

            services.AddControllers();
            
            services.AddGrpc();
            
            services.AddScoped<IMessageConsumer, Engine.BatchManager.BatchManager>();
            services.AddScoped<IBalticNode, Engine.BatchManager.BatchManager>();
            services.AddScoped<ITokens, Engine.BatchManager.BatchManager>(); // TODO verify
            services.AddScoped<IBalticServer, BalticServerProxy>(); // TODO verify
            services.AddScoped<ICluster, ClusterProxyProxy>(); // TODO verify
            services.AddSingleton<BatchManagerDbMock, BatchManagerDbMock>(); // TODO remove MOCK
            services.AddScoped<IDataModelImplFactory, DataModelImplFactory>();
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
                endpoints.MapGrpcService<CommonServices.NodeService>();
                endpoints.MapGrpcService<BalticNodeService>();

                endpoints.MapGet("/", async context =>
                {
                    await context.Response.WriteAsync("Communication with Baltic.Node endpoints must be made through a proper software. To learn about the software and project, visit: https://www.balticlsc.eu");
                });
                endpoints.MapControllers();
            });
        }        
    }
}