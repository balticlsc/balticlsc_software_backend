using Baltic.NetworkRegistry.DataAccess;
using Baltic.Types.DataAccess;
using Baltic.Web.Module;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Baltic.NetworkRegistry
{
    public class NetworkRegistryModule : IModule
    {
        public void AddModule(IServiceCollection services, IConfiguration configuration, IWebHostEnvironment environment = null)
        {
            services.AddScoped<INetworkManagement, NetworkManagementDaoImplMock>();
            services.AddScoped<INetworkBrokerage, NetworkBrokerageDaoImplMock>();
            services.AddSingleton<NetworkRegistryMock, NetworkRegistryMock>();
        }

        public void UseModule(IApplicationBuilder app)
        {
        }
    }
}