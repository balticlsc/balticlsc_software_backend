using System;

namespace Baltic.Consul.Exceptions
{
    public class ConsulKvStoreKeyNotFoundException : Exception
    {
        public string Key { set; get; }

        public ConsulKvStoreKeyNotFoundException()
        {
        }

        public ConsulKvStoreKeyNotFoundException(string key)
        {
            Key = key;
        }

        public ConsulKvStoreKeyNotFoundException(string key, string message) : base(message)
        {
            Key = key;
        }
    }
}