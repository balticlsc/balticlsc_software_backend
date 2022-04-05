using Grpc.Net.Client;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;

namespace Baltic.Node
{
    public static class MasterHostingWebHostBuilderExtensions
    {
        public static IWebHostBuilder UseMaster(
            this IWebHostBuilder hostBuilder,
            GrpcChannel masterChannel)
        {
            hostBuilder.ConfigureServices(services => services.AddSingleton(masterChannel));
            return hostBuilder;
        }
        
    }
}