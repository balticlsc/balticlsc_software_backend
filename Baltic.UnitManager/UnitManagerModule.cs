using Baltic.Types.DataAccess;
using Baltic.Web.Module;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Baltic.UnitManager
{
    public class UnitManagerModule : IModule
    {
        public void AddModule(IServiceCollection services, IConfiguration configuration, IWebHostEnvironment environment = null)
        {
            services.AddScoped<IUnitDevManager, Engine.UnitManager.UnitDevManager>();
        }

        public void UseModule(IApplicationBuilder app)
        {
        }
    }
}