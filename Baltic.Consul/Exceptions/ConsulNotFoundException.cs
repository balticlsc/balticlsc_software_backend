using System;

namespace Baltic.Consul.Exceptions
{
    public class ConsulNotFoundException : Exception
    {
        public Uri Uri { set; get; }

        public ConsulNotFoundException()
        {
        }

        public ConsulNotFoundException(Uri uri)
        {
            Uri = uri;
        }

        public ConsulNotFoundException(Uri uri, string message) : base(message)
        {
            Uri = uri;
        }
    }
}