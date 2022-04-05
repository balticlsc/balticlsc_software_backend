using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Baltic.Web.Module
{
    public interface IModule
    {
        public void AddModule(IServiceCollection services, IConfiguration configuration, IWebHostEnvironment environment = null);

        public void UseModule(IApplicationBuilder app);
    }
}