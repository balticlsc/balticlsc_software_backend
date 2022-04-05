using System;
using System.IO;
using System.Linq;
using System.Net.Http;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Serilog;
using Serilog.Core;
using Serilog.Events;
using Grpc.Net.Client;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.Extensions.DependencyInjection;


namespace Baltic.Node.ClusterProxy.Swarm
{
    internal static class Program
    {
        private static IConfiguration _configuration;
        private static string _environmentName;
        
        public static void Main(string[] args)
        {
            
            var httpHandler = new HttpClientHandler
            {
                ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator
            };

            try
            {
                var contentRootPath = Directory.GetCurrentDirectory();
                _environmentName = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Production";

                var builder = new ConfigurationBuilder()
                    .SetBasePath(contentRootPath)
                    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: false)
                    .AddJsonFile($"appsettings.{_environmentName}.json", optional: true, reloadOnChange: false)
                    .AddJsonFile($"appsettings.Local.json", optional: true, reloadOnChange: false)
                    .AddEnvironmentVariables();

                _configuration = builder.Build();

                var clusterProxyPort = int.Parse(_configuration["clusterProxyPort"]);
                var clusterProxyHost = _configuration["clusterProxyHost"];
                var clusterProxyChannel = GrpcChannel.ForAddress($"https://{clusterProxyHost}:{clusterProxyPort}",
                    new GrpcChannelOptions {HttpHandler = httpHandler});

                bool.TryParse(_configuration["Debug"], out var debugState);
                var logFile = _configuration["LogFile"];

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

                var app = CreateHostBuilder(args, _configuration, clusterProxyChannel).Build();
                Log.Information("ClusterProxy started. Press Ctrl+C to shut down.");
                app.Run();
            }
            catch (Exception ex)
            {
                Log.Fatal(ex, "ClusterProxy terminated unexpectedly");
            }
            finally
            {
                Log.CloseAndFlush();
            }
        }

        private static IHostBuilder CreateHostBuilder(string[] args, IConfiguration configuration, GrpcChannel channel) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureAppConfiguration((context, config) =>
                {
                    config.AddJsonFile("appsettings.Local.json", optional: true, reloadOnChange: false);
                })
                .ConfigureServices((hostContext, services) => { })
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder
                        .ConfigureKestrel(options =>
                        {
                            options.ListenAnyIP(int.Parse(configuration["clusterProxyPort"])-1, listenOptions =>
                            {
                               listenOptions.Protocols = HttpProtocols.Http1AndHttp2;
                            });
                            options.ListenAnyIP(int.Parse(configuration["clusterProxyPort"]), listenOptions =>
                            {
                                listenOptions.Protocols = HttpProtocols.Http1AndHttp2;
                                listenOptions.UseHttps();
                            });
                        })
                        .UseKestrel()
                        .UseSerilog()
                        .UseConfiguration(configuration)
                        .UseChannel(channel)
                        .UseStartup<Startup>();
                });
        
    }
    
    public static class MasterHostingWebHostBuilderExtensions
    {
        public static IWebHostBuilder UseChannel(
            this IWebHostBuilder hostBuilder,
            GrpcChannel channel)
        {
            hostBuilder.ConfigureServices(services => services.AddSingleton(channel));
            return hostBuilder;
        }
        
    }
}