using Baltic.TaskRegistry.DataAccess;
using Baltic.Types.DataAccess;
using Baltic.Web.Module;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Baltic.TaskRegistry
{
    public class TaskRegistryModule : IModule
    {
        public void AddModule(IServiceCollection services, IConfiguration configuration, IWebHostEnvironment environment = null)
        {
            services.AddSingleton<TaskRegistryMock, TaskRegistryMock>(); // TODO (remove in the final version of the TaskRegistry)
            services.AddScoped<ITaskManagement, TaskManagementDaoImplMock>(); // TODO (not Mock)
            services.AddScoped<ITaskProcessing, TaskProcessingDaoImplMock>(); // TODO (not Mock)
            services.AddScoped<ITaskBrokerage, TaskBrokerageDaoImplMock>(); // TODO (not Mock)
        }

        public void UseModule(IApplicationBuilder app)
        {
        }
    }
}