using System;
using Grpc.Net.Client;

namespace Baltic.CommonServices
{
    public class NodeItem
    {
        public string NodeId { get; set; }
        public string NodeName { get; set; }
        public string NodeUrl { get; set; }
        public DateTime LastSeen { get; set; }
        public GrpcChannel Channel { get; set; }
    }
}