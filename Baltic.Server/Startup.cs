using System;
using System.Collections.Generic;
using AutoMapper;
using Baltic.CalEditorRegistry;
using Baltic.CommonServices;
using Baltic.Consul;
using Baltic.DiagramRegistry;
using Baltic.NetworkManager;
using Baltic.NetworkRegistry;
using Baltic.Queue;
using Baltic.Security;
using Baltic.Server.Swagger;
using Baltic.TaskManager;
using Baltic.TaskManager.Controllers;
using Baltic.TaskRegistry;
using Baltic.UnitManager;
using Baltic.UnitRegistry;
using Baltic.UserRegistry;
using Baltic.Web.Extensions;
using Baltic.Web.Module;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.AspNetCore.Mvc.Versioning;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Net.Http.Headers;

namespace Baltic.Server
{
    
    public class Startup
    {
        private readonly string AllowServiceSpecificOrigins = "3cb37fdc-7d97-4280-ba7d-f0c566b3c84f";
        private IWebHostEnvironment Environment { get; }
        private IConfiguration Configuration { get; }
        
        public Startup(IWebHostEnvironment environment, IConfiguration configuration)
        {
            Environment = environment;
            Configuration = configuration;
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton(new NodeManager());
            services.Configure<ConsoleLifetimeOptions>(opts => { opts.SuppressStatusMessages = true; });

            // enable CORS if need - see appsettings.json
            if (Configuration != null && bool.TryParse(Configuration["Cors:Enable"], out var state))
            {
                if (state)
                {
                    var origins = Configuration.GetSection("").Get<List<string>>();
                    services.AddCors(options =>
                    {
                        options.AddPolicy(AllowServiceSpecificOrigins,
                            builder =>
                            {
                                builder
                                    .WithOrigins(origins.ToArray())
                                    .SetIsOriginAllowedToAllowWildcardSubdomains()
                                    .WithHeaders(HeaderNames.CacheControl)
                                    .AllowCredentials()
                                    .AllowAnyHeader()
                                    .AllowAnyMethod();
                            });
                    });
                }
            }

            services.AddAutoMapper(typeof(Startup));
            services.ConfigureAutoMapperForModules();            

            services.AddModule<SecurityModule>(Configuration, Environment);
            services.AddModule<ConsulModule>(Configuration);

            services.AddModule<UserRegistryModule>(Configuration);
            services.AddModule<NetworkRegistryModule>(Configuration);
            services.AddModule<TaskRegistryModule>(Configuration);
            services.AddModule<UnitRegistryModule>(Configuration);
            services.AddModule<DiagramRegistryModule>(Configuration);

            services.AddModule<TaskManagerModule>(Configuration);
            services.AddModule<UnitManagerModule>(Configuration);
            services.AddModule<NetworkManagerModule>(Configuration);
            services.AddModule<MultiQueueModule>(Configuration);

            // services.AddModule<BatchManagerModule>(Configuration);

            services.AddModule<CalEditorRegistryModule>(Configuration);            
            
            services.AddHealthChecks();

            services.AddMemoryCache();

            services.AddControllers();

            services.AddSignalR();

            services.AddGrpc();

            services.AddApiVersioning(config =>
            {
                config.DefaultApiVersion = new ApiVersion(1, 0);
                config.AssumeDefaultVersionWhenUnspecified = true;
                config.ReportApiVersions = true;
                config.ApiVersionReader = ApiVersionReader.Combine(
                    new HeaderApiVersionReader("X-version"));
                //new QueryStringApiVersionReader("api-version"));
            });
            services.AddVersionedApiExplorer(options =>
            {
                options.GroupNameFormat = "'v'VVV";
                options.SubstituteApiVersionInUrl = true;
            });

            services.AddSwaggerDocumentation(Environment.IsDevelopment());

            services.AddControllers().AddNewtonsoftJson();
            
            //services.AddAspNetIdentity<ApplicationUser>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IServiceProvider serviceProvider, IApiVersionDescriptionProvider apiVersionDescriptionProvider)
        {
            var httpContextAccessor = serviceProvider.GetService<IHttpContextAccessor>();
            
            app.UseRouting();
            
            // !!! must be loaded as first module !!!
            app.UseModule<SecurityModule>();            
            app.UseModule<ConsulModule>();

            app.UseModule<UserRegistryModule>();            
            app.UseModule<NetworkRegistryModule>();
            app.UseModule<TaskRegistryModule>();
            app.UseModule<UnitRegistryModule>();
            app.UseModule<DiagramRegistryModule>();
            
            app.UseModule<TaskManagerModule>();
            app.UseModule<NetworkManagerModule>();
            app.UseModule<UnitManagerModule>();
            app.UseModule<MultiQueueModule>();
            
            // app.UseModule<BatchManagerModule>();

            app.UseModule<CalEditorRegistryModule>();            
            
            if (Environment.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.ConfigureExceptionHandler();
                app.UseExceptionHandler("/Error");
            }
            
            app.UseStatusCodePages();

            // enable HTTPS if need - see appsettings.json 
            if (Configuration != null && bool.TryParse(Configuration["HTTPS:Enable"], out var httpsState))
            {
                if (httpsState)
                {
                    app.UseHttpsRedirection();
                }
            }            
            
            // enable CORS if need - see appsettings.json
            if (Configuration != null && bool.TryParse(Configuration["CORS:Enable"], out var corsState))
            {
                if (corsState)
                {
                    app.UseCors(AllowServiceSpecificOrigins);
                }
            }
           
            // enable Swagger if need - see appsettings.json
            app.UseSwaggerDocumentation(Configuration, apiVersionDescriptionProvider, Environment.IsDevelopment());
            
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapGrpcService<CommonServices.NodeService>();
                endpoints.MapGrpcService<BalticServerService>();
                
                endpoints.MapControllers();
            });
        }
    }
}
