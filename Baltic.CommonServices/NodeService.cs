using System.Threading.Tasks;
using Baltic.Core.Utils;
using Baltic.Types.Protos;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Microsoft.Extensions.Configuration;

namespace Baltic.CommonServices
{
    public class NodeService : NodeServiceApi.NodeServiceApiBase
    {
        private NodeManager Dictionary { get; }
        private IConfiguration Configuration { get; }
        
        public override Task<ResponseStatus> RegisterNode(RegisterRequest request, ServerCallContext context)
        {
            var item = new NodeItem()
            {
                NodeId = request.Id,
                NodeUrl = request.Url,
                NodeName = request.Name
            };
            Dictionary.RegisterOrUpdateNode(item.NodeId, item);
            return Task.FromResult(new ResponseStatus()
            {
                Code = ResponseStatus.Types.Codes.Ok
            });
        }

        public override Task<NodeStatus> GetNodeStatus(Empty e, ServerCallContext context)
        {
            var nodePort = int.Parse(Configuration["NodePort"]);
            
            return Task.FromResult(new NodeStatus()
            {
                Name = SystemInfo.MachineName,
                Id = SystemInfo.GetNodeId(),
                OsNameAndVersion = SystemInfo.OsNameAndVersion,
                UpTime = SystemInfo.UpTime,
                Url = ""
            });
        }
        
        public NodeService(NodeManager dict, IConfiguration configuration) : base()
        {
            Configuration = configuration;
            Dictionary = dict;
        }
    }
}