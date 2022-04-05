using Baltic.DiagramRegistry.DataAccess;
using Baltic.Types.DataAccess;
using Baltic.Web.Module;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Baltic.DiagramRegistry
{
    public class DiagramRegistryModule : IModule
    {
        public void AddModule(IServiceCollection services, IConfiguration configuration, IWebHostEnvironment environment = null)
        {
            services.AddScoped<IDiagram, DiagramDaoImplMock>(); // TODO (remove Mock)
        }

        public void UseModule(IApplicationBuilder app)
        {
        }
    }
}