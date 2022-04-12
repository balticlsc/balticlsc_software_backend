using System.Threading.Tasks;
using Grpc.Core;
using Baltic.ProtocolBuffers;
using Serilog;
using System.Collections.Concurrent;
using Baltic.Core.Utils;

namespace Baltic.Node.Services
{
    public class NodeItem
    {
        public string NodeId { get; set; }
        public string NodeName { get; set; }
        public string NodeUrl { get; set; }
        public string NodeUpTime { get; set; }
    }
    public class NodeService : NodeServiceApi.NodeServiceApiBase
    {
        public ConcurrentDictionary<string, NodeItem> Nodes;

        public override Task<ResponseStatus> RegisterNode(RegisterRequest request, ServerCallContext context)
        {

            Log.Information("Request from node: {name}", request.Name);
            return Task.FromResult(new ResponseStatus()
            {
                Code = ResponseStatus.Types.Codes.Ok
               
            });
        }

        public NodeService() : base()
        {
            Nodes = new ConcurrentDictionary<string, NodeItem>();
            var item = new NodeItem
            {
                NodeName = "",
                NodeId = SystemInfo.NodeId
            };

            Nodes.TryAdd(item.NodeId, item);
        }
    }
}
 