#pragma warning disable 1591
using System;
using System.IO;
using Baltic.Core.Utils;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Baltic.Node.Services;
using Baltic.Server.Extensions;
using Baltic.Server.Hubs;
using Microsoft.Net.Http.Headers;
using Microsoft.OpenApi.Models;

namespace Baltic.Server
{
    public class Startup
    {
        private readonly string AllowBalticSpecificOrigins = "3342c6b3-21fd-4603-b4c6-ad6d5d5c8c18";
        public IWebHostEnvironment Env { get; }
        public IConfiguration Configuration { get; }

        public Startup(IWebHostEnvironment env, IConfiguration configuration)
        {
            Env = env;
            Configuration = configuration;
        }


        public void ConfigureServices(IServiceCollection services)
        {
            services.Configure<ConsoleLifetimeOptions>(opts =>
            {
                opts.SuppressStatusMessages = true;
            });

            services.AddCors(options =>
            {
                options.AddPolicy(AllowBalticSpecificOrigins,
                    builder =>
                    {
                        builder
                            .WithOrigins("http://balticlsc.eu", "https://balticlsc.eu", Configuration["AllowBalticSpecificOrigin"] ?? "")
                            .SetIsOriginAllowedToAllowWildcardSubdomains()
                            .WithHeaders(HeaderNames.CacheControl)
                            .AllowCredentials()
                            .AllowAnyHeader()
                            .AllowAnyMethod();
                    });
            });


            services.AddMemoryCache();

            services.AddControllers();

            services.AddSignalR();

            services.AddGrpc();

            services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc("v1", new OpenApiInfo
                {
                    Version = "v1",
                    Title = "BalticLSC API",
                    Description = "Baltic Large-Scale Computing Service Api",
                    TermsOfService = new Uri("https://www.balticlsc.eu/terms"),
                    Contact = new OpenApiContact
                    {
                        Name = "BalticLSC",
                        Email = string.Empty,
                        Url = new Uri("https://www.balticlsc.eu/"),
                    },
                    License = new OpenApiLicense
                    {
                        Name = "Use under License",
                        Url = new Uri("https://www.balticlsc.eu/license"),
                    }
                });

                var xmlFile = $"{SystemInfo.ExecutingAssemblyName}.xml";
                var xmlPath = Path.Combine(SystemInfo.ContentRootPath, xmlFile);
                options.IncludeXmlComments(xmlPath);
            });
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            //if (env.IsDevelopment())
            //{
            //    app.UseDeveloperExceptionPage();
            //    //app.UseBrowserLink();
            //    app.ConfigureExceptionHandler();
            //}
            //else
            //{
            //    app.ConfigureExceptionHandler();
            //    app.UseExceptionHandler();
            //}+

            if (Configuration != null && bool.TryParse(Configuration["Https"], out var state))
            {
                if (state)
                {
                    app.UseHttpsRedirection();
                }
            }

            app.UseSwagger();

            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1");
                c.RoutePrefix = string.Empty;
            });

            app.UseRouting();
            app.UseCors(AllowBalticSpecificOrigins);
            //app.UseAuthentication();
            //app.UseAuthorization();
            app.UseStatusCodePages();
            app.ConfigureExceptionHandler();


            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapGrpcService<NodeService>();

                endpoints.MapHub<TimeMachineHub>("/timeMachine");
            });
        }
    }
}