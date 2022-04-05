using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using Baltic.Consul.Exceptions;
using Consul;

namespace Baltic.Consul
{
    public static class ConsulMethods
    {
        //It works only for our services with http health check.
        public static (string, int)? GetServiceAddress(string consulAddress, int consulPort, string serviceName)
        {
            var consulClient = new ConsulClient(config =>
            {
                config.Address = new Uri($"http://{consulAddress}:{consulPort}");
            });

            try
            {
                var servicesDictionary = consulClient.Agent.Services().Result.Response;
                var keyForService = servicesDictionary.Keys.FirstOrDefault(k => k.Contains(serviceName));

                if (null == keyForService)
                {
                    return null;
                }

                var serviceAddress = servicesDictionary[keyForService].Address;
                var servicePort = servicesDictionary[keyForService].Port;

                if (!IsServiceAlive(serviceAddress, servicePort))
                {
                    return null;
                }

                return (serviceAddress, servicePort);
            }
            catch (Exception)
            {
                throw new ConsulNotFoundException();
            }
        }

        //It works only for our services with http health check.
        public static bool IsServiceAlive(string serviceAddress, int servicePort, bool https = false)
        {
            var httpClient = new HttpClient();
            HttpStatusCode status;
            var requestUri = https
                ? $"https://{serviceAddress}:{servicePort}/api/health/status"
                : $"http://{serviceAddress}:{servicePort}/api/health/status";

            try
            {
                status = httpClient.GetAsync(requestUri).Result.StatusCode;
            }
            catch (Exception)
            {
                return false;
            }

            return status == HttpStatusCode.OK;
        }
    }
}