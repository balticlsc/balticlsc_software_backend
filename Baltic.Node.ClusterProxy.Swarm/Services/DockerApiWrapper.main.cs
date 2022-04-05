using System;
using System.Linq;
using System.Text;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Threading;
using System.Threading.Tasks;
using Baltic.Core.Extensions;
using Baltic.Types.Protos;
using Docker.DotNet;
using Docker.DotNet.Models;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json.Linq;
using Serilog;

namespace Baltic.Node.ClusterProxy.Swarm.Services
{
    public partial class DockerApiWrapper : ISwarmProxy
    {
        private const string DockerStackLabel = "com.docker.stack.namespace";
        private const string DockerServiceNameLabel = "com.docker.swarm.service.name";
        private const string BalticModuleLabel = "BalticLSC.ModuleId";
        private const string BalticSystemNetworkName = "BalticLSC_System";
        private const string BalticBatchManagerNetworkName = "BalticLSC_BatchManager";

        private DockerClient _dockerClient { get; }
        private string _projectPrefix { get; }
        private string _batchManagerNetworkName { get; }
        
        public DockerApiWrapper(IConfiguration configuration)
        {
            _projectPrefix = configuration["projectPrefix"];
            _batchManagerNetworkName = configuration["batchManagerNetworkName"];
            _dockerClient = new DockerClientConfiguration().CreateClient();
        }
    }
}