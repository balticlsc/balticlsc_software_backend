using Baltic.Engine.JobBroker;
using Baltic.Engine.TaskManager;
using Baltic.Engine.TaskProcessor;
using Baltic.TaskManager.Proxies;
using Baltic.Types.Entities;
using Baltic.Web.Module;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Baltic.TaskManager
{
    public class TaskManagerModule : IModule
    {
        public void AddModule(IServiceCollection services, IConfiguration configuration, IWebHostEnvironment environment = null)
        {
            services.AddScoped<IDataModelImplFactory, DataModelImplFactory>();
            services.AddScoped<IClusterNodeAccessFactory, ClusterNodeAccessFactory>(); // TODO resolve this mock
            services.AddScoped<IJobBroker, JobBroker>();
            services.AddScoped<ITaskProcessor, TaskProcessor>();
            services.AddScoped<ITaskManager, Engine.TaskManager.TaskManager>();
            services.AddScoped<ICALTranslation, Engine.TaskManager.TaskManager>();
            //services.AddScoped<Mapper, Mapper>(); // TODO - move to the proper project
        }

        public void UseModule(IApplicationBuilder app)
        {
            //app.UseEndpoints(endpoints =>
            //{
            //    endpoints.MapGrpcService<SwarmCommunicationService>();
            //});
        }
    }
}