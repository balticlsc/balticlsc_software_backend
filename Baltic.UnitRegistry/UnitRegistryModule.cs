using Baltic.Types.DataAccess;
using Baltic.UnitRegistry.DataAccess;
using Baltic.Web.Module;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Baltic.UnitRegistry
{
    public class UnitRegistryModule : IModule
    {
        public void AddModule(IServiceCollection services, IConfiguration configuration, IWebHostEnvironment environment = null)
        {
            services.AddScoped<IUnitProcessing, UnitProcessingDaoImplMock>(); // TODO (not Mock)
            services.AddScoped<IUnitManagement, UnitManagementDaoImplMock>(); // TODO (not Mock)
            services.AddSingleton<UnitRegistryMock, UnitRegistryMock>(); // TODO (remove in the final version of the TaskRegistry)
        }

        public void UseModule(IApplicationBuilder app)
        {
        }
    }
}