using Baltic.Queue.MultiQueue;
using Baltic.Types.QueueAccess;
using Baltic.Web.Module;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Baltic.Queue
{
    public class MultiQueueModule : IModule
    {
        public void AddModule(IServiceCollection services, IConfiguration configuration, IWebHostEnvironment environment = null)
        {
            services.AddSingleton<MultiQueueDbMock, MultiQueueDbMock>(); // TODO (remove Mock)
            services.AddScoped<IMessageProcessing, MultiQueue.MultiQueue>();
            services.AddScoped<IMessageBrokerage, MultiQueue.MultiQueue>();
        }

        public void UseModule(IApplicationBuilder app)
        {
        }
    }
}