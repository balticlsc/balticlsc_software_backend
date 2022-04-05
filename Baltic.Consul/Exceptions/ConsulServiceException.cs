using System;

namespace Baltic.Consul.Exceptions
{
    public sealed class ConsulServiceException : Exception
    {
        public string HostName { get; }
        
        public ConsulServiceException(string hostName) : this(string.Empty, hostName)
        {
        }

        public ConsulServiceException(string message, string hostName) : base(message)
        {
            HostName = hostName;
        }
    }
}