using Baltic.Node.Services;
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
            services.Configure<ConsoleLifetimeOptions>(opts =>
            {
                opts.SuppressStatusMessages = true;
            });

            services.AddGrpc();
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
                endpoints.MapGrpcService<NodeService>();

                endpoints.MapGet("/", async context =>
                {
                    await context.Response.WriteAsync("Communication with Baltic.Node endpoints must be made through a proper software. To learn about the software and project, visit: https://www.balticlsc.eu");
                });
            });
        }
    }
}
