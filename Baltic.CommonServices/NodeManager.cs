using System;
using System.Collections.Concurrent;
using System.Net.Http;
using Grpc.Net.Client;
using Serilog;

namespace Baltic.CommonServices
{
    public class NodeManager : ConcurrentDictionary<string, NodeItem>
    {
        public NodeManager() : base()
        {
        }

        public bool RegisterOrUpdateNode(string key, NodeItem value)
        {
            if (TryGetValue(key, out var val))
            {
                val.LastSeen = DateTime.UtcNow;
                Log.Debug("Update node: {id}({name})", value.NodeId, value.NodeName);                
                return true;
            } 
            Log.Debug("Register node: {id}({name}) at position: {cnt}", value.NodeId, value.NodeName, Count+1);
            return base.TryAdd(key, value);
        }

        public void RemoveOld()
        {
            foreach (var val in Values)
            {
                var offset = DateTime.UtcNow - val.LastSeen;
                if (offset.TotalMinutes > 15)
                {
                    if (TryRemove(val.NodeId, out var removed))
                    {
                        Log.Information("Unregister node: {node}", removed.NodeId);
                    }
                }
            }
        }

        public T GetClient<T>(string id)
        {
            var client = default(T);
            
            if (TryGetValue(id, out var node))
            {
                var httpHandler = new HttpClientHandler
                {
                    ServerCertificateCustomValidationCallback =
                        HttpClientHandler.DangerousAcceptAnyServerCertificateValidator
                };

                if (null == node.Channel) // TODO - change NodeItem to a real singleton
                {
                    Log.Debug($"Creating gRPC channel for node (ID: {node.NodeId} URL: {node.NodeUrl})");
                    node.Channel =
                        GrpcChannel.ForAddress(node.NodeUrl, new GrpcChannelOptions {HttpHandler = httpHandler});
                }
                client = (T) Activator.CreateInstance(typeof(T), node.Channel);
            }

            return client;
        }
    }
}