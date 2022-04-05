using System;
using System.Collections.Generic;
using AutoMapper;
using Baltic.Database;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Serilog;

namespace Baltic.Web.Module
{
    public static class ModuleExtensions
    {
        private static List<Type> _moduleList = new List<Type>();
        public static void AddAutoMapperForModules()
        {
            
        }
        public static void AddModule<T>(this IServiceCollection services, IConfiguration configuration) where T : class, IModule, new()
        {
            AddModule<T>(services, configuration, null);
        }
        
        public static void AddModule<T>(this IServiceCollection services, IConfiguration configuration, IWebHostEnvironment environment) where T : class, IModule, new()
        {
            Log.Information("Initialize module: {moduleName}", typeof(T).Namespace);
            var module = new T();
            DB.AddMigrations(typeof(T));
            _moduleList.Add(typeof(T));
            services.AddSingleton(module);
            module.AddModule(services, configuration, environment);
        }

        public static void ConfigureAutoMapperForModules(this IServiceCollection services)
        {
            try
            {
                var mappingConfig = new MapperConfiguration(cfg =>
                {
                    cfg.ShouldMapField = mf => false;
                    cfg.ShouldMapProperty = mp => true;
                    cfg.ShouldMapMethod = mm => false;
                    
                    cfg.AddMaps(_moduleList.ToArray());
                });
                mappingConfig.CompileMappings();
                mappingConfig.AssertConfigurationIsValid();
                
                var mapper = mappingConfig.CreateMapper();
                services.AddSingleton(mapper);
                Log.Debug("Configure Auto Mapper for modules... {status}", "done");
            }
            catch (Exception e)
            {
                Log.Error("Auto Mapper problem: {msg}", e.Message);
            }
        }

        public static void UseModule<T>(this IApplicationBuilder app) where T : IModule
        {
            var serviceProvider = app.ApplicationServices;
            var module = serviceProvider.GetService<T>();
            module.UseModule(app);
        }
    }
}