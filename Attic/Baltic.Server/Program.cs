using System;
using System.IO;
using System.Threading.Tasks;
using System.Reflection;
using System.Runtime;
using System.Runtime.Versioning;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Serilog;
using Baltic.Core.Utils;
using Baltic.Server.Database;

namespace Baltic.Server
{
    internal class Program
    {
        private static void OnApplicationStarted()
        {
            var contentRootPath = Directory.GetCurrentDirectory();
            var runtimeVersion = (Assembly.GetEntryAssembly() ?? throw new InvalidOperationException()).GetCustomAttribute<TargetFrameworkAttribute>().FrameworkName;
            var osNameAndVersion = System.Runtime.InteropServices.RuntimeInformation.OSDescription;
            var machineName = Environment.MachineName;

            var isServerGC = GCSettings.IsServerGC;
            var largeObjectHeapCompactionMode = GCSettings.LargeObjectHeapCompactionMode;
            var latencyMode = GCSettings.LatencyMode;
            var nodeId = SystemInfo.GetNodeId();

            Console.Title = string.Format($"Server - {machineName} - Node: {nodeId}");
            Log.Information("Staring BaltiLSC Server Node on Machine: {Machine}, node id: {Id}", machineName, nodeId);
            Log.Information("Runtime Version: {Version}, OS: {OS}", runtimeVersion, osNameAndVersion);
            Log.Information("Runtime Settings: GC Server: {isGCServer}, GC LOH Mode: {LOHCompact}, GC Latency Mode: {LatencyMode}", isServerGC, largeObjectHeapCompactionMode, latencyMode);
            Log.Information("Using configuration from directory: {directory}", contentRootPath);
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
                                options.ListenAnyIP(8050, listenOptions =>
                                {
                                    listenOptions.Protocols = HttpProtocols.Http1AndHttp2;
                                });
                                //options.ListenAnyIP(5001, listenOptions =>
                                //{
                                //    listenOptions.Protocols = HttpProtocols.Http1AndHttp2;
                                //    //listenOptions.UseHttps();
                                //});
                            })
                            .UseKestrel()
                            .UseConfiguration(configuration)
                            .UseStartup<Startup>();
                    });

        public static async Task Main(string[] args)
        {
            try
            {
                var contentRootPath = Directory.GetCurrentDirectory();
                var environmentName = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Production";

                var builder = new ConfigurationBuilder()
                    .SetBasePath(contentRootPath)
                    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                    .AddJsonFile($"appsettings.{environmentName}.json", optional: true)
                    .AddCommandLine(args)
                    .AddEnvironmentVariables()
                    .AddUserSecrets<Startup>();

                var configuration = builder.Build();

                bool.TryParse(configuration["Debug"], out var debugState);
                var logFile = configuration["LogFile"];
                LoggerSetup.CreateLogger(debugState, logFile);
                OnApplicationStarted();

                var connectionString = configuration.GetConnectionString("DefaultConnection");
                if (connectionString != "")
                {
                    DB.ConnectionString = connectionString;
                    DB.AddMigrations(typeof(Program));
                    Log.Information("Database: {version} Schema version: {schema}", DB.Version, DB.SchemaVersion);
                }

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
