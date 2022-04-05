using System;
using Baltic.Core.Utils;

namespace Baltic.Consul
{
    public class ConsulOptions
    {
        private string _host;
        public bool Enabled { get; set; }

        public string Host
        {
            get
            {
                if (SystemInfo.Windows)
                {
                    return "http://localhost";
                }
                return _host;
            }
            set => _host = value;
        }

        public int Port { get; set;  }
        public string ServiceName { get; set;  }
        public int ServicePort { get; set;  }
        public int ServiceHealthCheckTimeOut { get; set;  }
        public int ServiceHealthCheckInterval { get; set; }
        public int DeregisterCriticalServiceAfter { get; set; }
        public bool CheckService { get; set; }
        public bool TLSSkipVerify { get; set; }
        public string Id { get; set; }
        
        public bool UseSSL { get; set; }
        public Uri ConsulAddress {get => new Uri($"{Host}:{Port}");}

        public bool IsValid()
        {
            return !(string.IsNullOrEmpty(Host) || Port == 0);
        }

        public ConsulOptions()
        {
            Enabled = false;
            CheckService = false;
        }
    }
}