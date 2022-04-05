using Baltic.Web.Module;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Baltic.CalEditorRegistry
{
    public class CalEditorRegistryModule : IModule
    {
        public void AddModule(IServiceCollection services, IConfiguration configuration, IWebHostEnvironment environment = null)
        {
        }

        public void UseModule(IApplicationBuilder app)
        {
        }
    }
}