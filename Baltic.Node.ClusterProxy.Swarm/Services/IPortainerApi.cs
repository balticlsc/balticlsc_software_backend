using System.Collections.Generic;
using System.Threading.Tasks;
using Docker.DotNet.Models;
using Newtonsoft.Json.Linq;

namespace Baltic.Node.ClusterProxy.Swarm.Services
{
    public interface IPortainerApi
    {
        public Task<IEnumerable<ContainerListResponse>> ContainerGetByModuleId(string moduleId);
        public Task<string> CustomCall(string path);
        public string GetPortainerEndpointsId();
    }
    
}