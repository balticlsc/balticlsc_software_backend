using System;
using Grpc.Net.Client;
using Baltic.ProtocolBuffers;
using Grpc.Core;

namespace RegisterToNode
{
    class Program
    {
        static void Main(string[] args)
        {
            // The port number(5001) must match the port of the gRPC server.
            var channel = GrpcChannel.ForAddress("https://localhost:5001", new GrpcChannelOptions {
                ThrowOperationCanceledOnCancellation = true

            });
            var client = new NodeServiceApi.NodeServiceApiClient(channel);
            var reply = client.RegisterNode(new RegisterRequest
            {
                Name = "Test"
            });
            Console.ReadKey();
        }
    }
}
