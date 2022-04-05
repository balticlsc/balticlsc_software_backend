using System;
using System.IO;
using System.Reflection;
using System.Runtime;
using System.Runtime.Versioning;
using System.Threading.Tasks;
using Baltic.Core.Utils;
using Baltic.Database;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Serilog;
using Serilog.Core;
using Serilog.Events;

namespace Baltic.Server
{
    public static class Program
    {
        private static void OnApplicationStarted()
        {
            var contentRootPath = Directory.GetCurrentDirectory();
            var runtimeVersion = (Assembly.GetEntryAssembly() ?? throw new InvalidOperationException()).GetCustomAttribute<TargetFrameworkAttribute>()?.FrameworkName;
            var osNameAndVersion = System.Runtime.InteropServices.RuntimeInformation.OSDescription;
            var machineName = Environment.MachineName;

            var isServerGc = GCSettings.IsServerGC;
            var largeObjectHeapCompactionMode = GCSettings.LargeObjectHeapCompactionMode;
            var latencyMode = GCSettings.LatencyMode;
            var nodeId = SystemInfo.GetNodeId();

            Console.Title = string.Format($"Server - {machineName} - Node: {nodeId}");
            Log.Information("Staring BaltiLSC Server Node on Machine: {Machine}, node id: {Id}", machineName, nodeId);
            Log.Information("Runtime Version: {Version}, OS: {OS}", runtimeVersion, osNameAndVersion);
            Log.Information("Runtime Settings: GC Server: {isGCServer}, GC LOH Mode: {LOHCompact}, GC Latency Mode: {LatencyMode}", isServerGc, largeObjectHeapCompactionMode, latencyMode);
            Log.Information("Using configuration from directory: {directory}", contentRootPath);
            Log.Information("Host name: {hostName}, ip: {ip}", SystemInfo.GetHostName(), SystemInfo.GetIpAddress());
        }        
        // call on program exit
        private static void OnExit(object sender, EventArgs eventArgs)
        {
            
        }
        
        // call on unhandled exception
        private static void OnUnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            var ex = (Exception)e.ExceptionObject;
            Console.WriteLine($"Unhandled Exception: {ex.ToString()}");
            Environment.Exit(System.Runtime.InteropServices.Marshal.GetHRForException(ex));
        }

        private static IHostBuilder CreateHostBuilder(string[] args, IConfiguration configuration) =>
            Host.CreateDefaultBuilder(args)
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
                            //options.ListenAnyIP(5000, listenOptions =>
                            //{
                            //   listenOptions.Protocols = HttpProtocols.Http2;
                            //});
                            options.ListenAnyIP(5001, listenOptions =>
                            {
                                listenOptions.Protocols = HttpProtocols.Http1AndHttp2;
                                //listenOptions.UseHttps("wut.pfx", "Pa$$w0rd");
                                listenOptions.UseHttps();
                            });
                        })
                        .UseKestrel()                        
                        .UseConfiguration(configuration)                        
                        .UseStartup<Startup>();
                });
        
        public static async Task Main(string[] args)
        {
            AppDomain.CurrentDomain.UnhandledException += OnUnhandledException;
            AppDomain.CurrentDomain.ProcessExit += OnExit;

            try
            {
                var contentRootPath = Directory.GetCurrentDirectory();
                var environmentName = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Production";

                var builder = new ConfigurationBuilder()
                    .SetBasePath(contentRootPath)
                    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                    .AddJsonFile($"appsettings.{environmentName}.json", optional: true)
                    .AddJsonFile($"appsettings.local.json", optional: true)                    
                    .AddCommandLine(args)
                    .AddEnvironmentVariables()
                    .AddUserSecrets<Startup>();

                var configuration = builder.Build();
                
                bool.TryParse(configuration["Debug"], out var debugState);
                var logFile = configuration["LogFile"];

                var loggingLevelSwitch = new LoggingLevelSwitch
                {
                    MinimumLevel = debugState ? LogEventLevel.Debug : LogEventLevel.Information
                };

                Log.Logger = new LoggerConfiguration()
                    .MinimumLevel.ControlledBy(loggingLevelSwitch)
                    .MinimumLevel.Override("Microsoft", LogEventLevel.Information)
                    .MinimumLevel.Override("Microsoft.AspNetCore", LogEventLevel.Warning)                     
                    .Enrich.FromLogContext()
                    .WriteTo.Console()
                    .WriteTo.RollingFile(logFile)
                    .CreateLogger();

                OnApplicationStarted();
                
                var connectionString = configuration.GetConnectionString("DefaultConnection");
                if (connectionString != "")
                {
                    DB.ConnectionString = connectionString;
                    Log.Information("Database server: {version}", DB.Version);
                    Log.Information("Initialize main database schema for: {name}", typeof(Program).Namespace);
                    DB.AddMigrations(typeof(Program));                    
                }

                var app = CreateHostBuilder(args, configuration).Build();

                Log.Information("Database schema version: {schema}", DB.SchemaVersion);
                Log.Information("BalticLSC Server started. Press Ctrl+C to shut down.");                
                await app.RunAsync();
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
