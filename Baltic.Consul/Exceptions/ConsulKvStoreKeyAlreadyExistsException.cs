using System;

namespace Baltic.Consul.Exceptions
{
    public class ConsulKvStoreKeyAlreadyExistsException : Exception
    {
        public string Key { set; get; }
        
        public ConsulKvStoreKeyAlreadyExistsException()
        {
        }

        public ConsulKvStoreKeyAlreadyExistsException(string key)
        {
            Key = key;
        }

        public ConsulKvStoreKeyAlreadyExistsException(string key, string message) : base(message)
        {
            Key = key;
        }
    }
}