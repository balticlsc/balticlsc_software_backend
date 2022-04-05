using System;
using System.Diagnostics;
using System.IO;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Baltic.Core.Utils;
using Baltic.Types.Protos;
using Grpc.Net.Client;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Serilog;
using Serilog.Core;
using Serilog.Events;
using IConfiguration = Microsoft.Extensions.Configuration.IConfiguration;

namespace Baltic.Node
{
    internal static class Program
    {
        private static readonly CancellationTokenSource TokenSource = new CancellationTokenSource();
        private static bool _registered = false;
        private static IConfiguration _configuration;
        private static string _environmentName;
        private static string _nodeId;

        private static bool IsDevelopment()
        {
            return _environmentName.ToUpper().Equals(Environments.Development.ToUpper());
        }        
        
        private static void OnApplicationStarted()
        {
            Console.Title = string.Format($"Server - {SystemInfo.MachineName} on node: {_nodeId}");
            Log.Information("Staring BaltiLSC Server Node on Machine: {Machine}, node id: {Id}", SystemInfo.MachineName, _nodeId);
            Log.Information("Runtime Version: {Version}, OS: {OS}", SystemInfo.RuntimeVersion, SystemInfo.OsNameAndVersion);
            Log.Information("Runtime Settings: GC Server: {isGCServer}, GC LOH Mode: {LOHCompact}, GC Latency Mode: {LatencyMode}", SystemInfo.IsServerGC, SystemInfo.LargeObjectHeapCompactionMode, SystemInfo.LatencyMode);
            Log.Information("Using configuration from directory: {directory}", SystemInfo.ContentRootPath);
            if (IsDevelopment())
            {
                Log.Information("Development mode: {mode}", IsDevelopment());
            }
        }

        private static void HealthService(CancellationToken token, GrpcChannel masterChannel)
        {
            //if (masterChannel == null) throw new ArgumentNullException(nameof(masterChannel));
            
            var masterPort = int.Parse(_configuration["MasterPort"]);
            var masterHost = _configuration["MasterHost"];
            var nodePort = int.Parse(_configuration["NodePort"]);
            var nodePublicHost = _configuration["NodePublicHost"];
            
            Task.Factory.StartNew( async () =>
            {
                var latencyWatch = new Stopwatch();

                while (true)
                {
                    if (token.IsCancellationRequested)
                    {
                        Log.Information("Unregister service");
                        break;
                    }

                    if (!_registered)
                    {
                        _registered = true;
                        Log.Information("Register the node on the master system {host}:{port}", masterHost, masterPort);
                    }

                    try
                    { 
                        var client = new NodeServiceApi.NodeServiceApiClient(masterChannel);
                        
                        latencyWatch.Restart();
                        var reply = client.RegisterNode(new RegisterRequest()
                        {
                            Id = _nodeId,
                            Ip = SystemInfo.GetIpAddress(),
                            Name = SystemInfo.MachineName,
                            Url = nodePublicHost 
                        });
                        latencyWatch.Stop();
                        var latency = latencyWatch.ElapsedMilliseconds;
                        Log.Logger.Information("Sending register client message to check connectivity. Last Ping Was: {ping} ", latency);
                    }
                    catch (Exception e)
                    {
                        Log.Error("Heartbeat error message is: {0}", e.Message);
                    }
                    finally
                    {
                        var shutdownTask = masterChannel.ShutdownAsync();
                        shutdownTask.Wait(token);
                    }                    
                    await Task.Delay(10000, token);
                }
            }, token);
        }
        
        // call on program exit
        private static void OnExit(object sender, EventArgs eventArgs)
        {
            TokenSource.Cancel();
        }        
        
        private static IHostBuilder CreateHostBuilder(string[] args, IConfiguration configuration, GrpcChannel masterChannel) =>
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
                            var nodePort = int.Parse(_configuration["NodePort"]);
                            options.ListenAnyIP(nodePort, listenOptions =>
                            {
                                listenOptions.Protocols = HttpProtocols.Http2;
                                listenOptions.UseHttps();
                            });
                            options.ListenAnyIP(7000, listenOptions =>
                            {
                                listenOptions.Protocols = HttpProtocols.Http1AndHttp2;
                            });
                        })
                        .UseKestrel()
                        .UseConfiguration(configuration)
                        .UseMaster(masterChannel)
                        .UseStartup<Startup>();
                });
        
        public static async Task Main(string[] args)
        {
            AppDomain.CurrentDomain.ProcessExit += OnExit;

            var httpHandler = new HttpClientHandler
            {
                ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator
            };            
            
            try { 
                var contentRootPath = Directory.GetCurrentDirectory();
                _environmentName = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Production";
               
                var builder = new ConfigurationBuilder()
                    .SetBasePath(contentRootPath)
                    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                    .AddJsonFile($"appsettings.{_environmentName}.json", optional: true)                    
                    .AddJsonFile($"appsettings.local.json", optional: true)
                    .AddEnvironmentVariables();

                _configuration = builder.Build();

                var masterPort = int.Parse(_configuration["MasterPort"]);
                var masterHost = _configuration["MasterHost"];
                var masterChannel = GrpcChannel.ForAddress($"https://{masterHost}:{masterPort}", new GrpcChannelOptions { HttpHandler = httpHandler });

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
                
                var nodePort = int.Parse(_configuration["NodePort"]);
                _nodeId = SystemInfo.GetNodeId(nodePort);
                if (IsDevelopment())
                {
                    AppContext.SetSwitch(
                        "System.Net.Http.SocketsHttpHandler.Http2UnencryptedSupport", true);
                    var tmpNodeId = _configuration["NodeId"];
                    if (!string.IsNullOrEmpty(tmpNodeId))
                    {
                        _nodeId = _configuration["NodeId"];
                    }
                }
                
                OnApplicationStarted();

                HealthService(TokenSource.Token, masterChannel);                
                var app = CreateHostBuilder(args, _configuration, masterChannel).Build();
                Log.Information("BalticLSC Node started. Press Ctrl+C to shut down.");
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