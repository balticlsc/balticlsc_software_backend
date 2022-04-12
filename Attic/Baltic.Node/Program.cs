using System;
using System.IO;
using System.Reflection;
using System.Runtime;
using System.Runtime.Versioning;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Serilog;
using Baltic.Core.Utils;

namespace Baltic.Node
{
    internal class Program
    {
        private static void OnApplicationStarted()
        {
            Console.Title = string.Format($"Server - {SystemInfo.MachineName} on node: {SystemInfo.NodeId}");
            Log.Information("Staring BaltiLSC Server Node on Machine: {Machine}, node id: {Id}", SystemInfo.MachineName, SystemInfo.NodeId);
            Log.Information("Runtime Version: {Version}, OS: {OS}", SystemInfo.RuntimeVersion, SystemInfo.OsNameAndVersion);
            Log.Information("Runtime Settings: GC Server: {isGCServer}, GC LOH Mode: {LOHCompact}, GC Latency Mode: {LatencyMode}", SystemInfo.IsServerGC, SystemInfo.LargeObjectHeapCompactionMode, SystemInfo.LatencyMode);
            Log.Information("Using configuration from directory: {directory}", SystemInfo.ContentRootPath);
        }

        public static IHostBuilder CreateHostBuilder(string[] args, IConfiguration configuration) =>
                Host.CreateDefaultBuilder(args)
                    .UseWindowsService()
                    .ConfigureAppConfiguration((context, config) =>
                    {
                    })
                    .ConfigureServices((hostContext, services) =>
                    {
                    })
                    .ConfigureWebHostDefaults(webBuilder =>
                    {
                        webBuilder
                            .UseUrls()
                            .UseSerilog()
                            .ConfigureKestrel(options =>
                            {
                                options.ListenLocalhost(5000, listenOptions =>
                                {
                                    listenOptions.Protocols = HttpProtocols.Http1AndHttp2;
                                });
                                options.ListenAnyIP(5001, listenOptions =>
                                {
                                    listenOptions.Protocols = HttpProtocols.Http1AndHttp2;
                                    listenOptions.UseHttps();
                                });
                            })
                            .UseKestrel()
                            .UseConfiguration(configuration)
                            .UseStartup<Startup>();
                    });


        public static async Task Main(string[] args)
        {
            try { 
                var contentRootPath = Directory.GetCurrentDirectory();
                var environmentName = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Production";
                var builder = new ConfigurationBuilder()
                    .SetBasePath(contentRootPath)
                    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                    .AddJsonFile($"appsettings.{environmentName}.json", optional: true)
                    .AddEnvironmentVariables();

                IConfiguration configuration = builder.Build();

                bool.TryParse(configuration["Debug"], out var debugState);
                var logFile = configuration["LogFile"];
                LoggerSetup.CreateLogger(debugState, logFile);
                OnApplicationStarted();

                Log.Information("Application started. Press Ctrl+C to shut down.");
                await CreateHostBuilder(args, configuration).Build().RunAsync();
            }
            catch (Exception ex)
            {
                Log.Fatal(ex, "Host terminated unexpectedly");
            }
            finally
            {
                Log.CloseAndFlush();
            }
        }
    }
}
