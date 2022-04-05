using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Core;
using Serilog.Events;

namespace Baltic.Node.ModuleManager
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var contentRootPath = Directory.GetCurrentDirectory();

            var builder = new ConfigurationBuilder()
                .SetBasePath(contentRootPath)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddEnvironmentVariables();

            var configuration = builder.Build();

            bool.TryParse(configuration["Debug"], out var debugState);
            var loggingLevelSwitch = new LoggingLevelSwitch
            {
                MinimumLevel = debugState ? LogEventLevel.Debug : LogEventLevel.Information
            };

            var logFile = configuration["LogFile"];

            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.ControlledBy(loggingLevelSwitch)
                .MinimumLevel.Override("Microsoft", LogEventLevel.Information)
                .MinimumLevel.Override("Microsoft.AspNetCore", LogEventLevel.Warning)
                .Enrich.FromLogContext()
                .WriteTo.Console()
                .WriteTo.RollingFile(logFile)
                .CreateLogger();

            Log.Information("Starting BalticLSC ModuleManager");
            var app = CreateHostBuilder(args, configuration).Build();
            app.Run();
        }
        public static IHostBuilder CreateHostBuilder(string[] args, IConfiguration configuration) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder
                        .ConfigureKestrel(options =>
                        {
                            options.ListenAnyIP(int.Parse(configuration["ModuleManagerPort"])-1, listenOptions =>
                            {
                                listenOptions.Protocols = HttpProtocols.Http1AndHttp2;
                            });
                            options.ListenAnyIP(int.Parse(configuration["ModuleManagerPort"]), listenOptions =>
                            {
                                listenOptions.Protocols = HttpProtocols.Http1AndHttp2;
                                //listenOptions.UseHttps("wut.pfx", "Pa$$w0rd");
                                listenOptions.UseHttps();
                            });
                        })
                        .UseKestrel()
                        .UseSerilog()
                        .UseConfiguration(configuration)
                        .UseStartup<Startup>();
                });
    }
}