using System;
using Baltic.Consul.Exceptions;
using Baltic.Core.Utils;
using Consul;

namespace Baltic.Consul
{
    public static class ConsulKvMethods
    {
        public static void CrateKeyValueElement(string key, object value, Uri consulUri)
        {
            var consulClient = new ConsulClient(config => { config.Address = consulUri; });

            if (null == key)
            {
                throw new ArgumentException("Key cannot be null.");
            }

            if (null == value)
            {
                throw new ArgumentException("Value cannot be null.");
            }

            try
            {
                if (null != consulClient.KV.Get(key).Result.Response)
                {
                    throw new ConsulKvStoreKeyAlreadyExistsException(key);
                }

                var pair = new KVPair(key)
                {
                    Value = ObjectFormatters.ObjectToByteArray(value)
                };

                var tmp = consulClient.KV.Put(pair).Result;
            }
            catch (ConsulKvStoreKeyAlreadyExistsException)
            {
                throw;
            }
            catch (Exception)
            {
                throw new ConsulNotFoundException(consulUri);
            }
        }

        public static object GetKeyValueElement(string key, Uri consulUri)
        {
            var consulClient = new ConsulClient(config => { config.Address = consulUri; });

            if (null == key)
            {
                throw new ArgumentException("Key cannot be null.");
            }

            try
            {
                var value = consulClient.KV.Get(key).Result.Response;

                if (null == value)
                {
                    throw new ConsulKvStoreKeyNotFoundException(key);
                }

                return ObjectFormatters.ObjectFromByteArray(value.Value);
            }
            catch (ConsulKvStoreKeyNotFoundException)
            {
                throw;
            }
            catch (Exception)
            {
                throw new ConsulNotFoundException(consulUri);
            }
        }

        public static void UpdateKeyValueElement(string key, object value, Uri consulUri)
        {
            var consulClient = new ConsulClient(config => { config.Address = consulUri; });

            if (null == key)
            {
                throw new ArgumentException("Key cannot be null.");
            }

            if (null == value)
            {
                throw new ArgumentException("Value cannot be null.");
            }

            try
            {
                if (null == consulClient.KV.Get(key).Result.Response)
                {
                    throw new ConsulKvStoreKeyNotFoundException(key);
                }

                var pair = new KVPair(key)
                {
                    Value = ObjectFormatters.ObjectToByteArray(value)
                };

                var tmp = consulClient.KV.Put(pair).Result;
            }
            catch (ConsulKvStoreKeyNotFoundException)
            {
                throw;
            }
            catch (Exception)
            {
                throw new ConsulNotFoundException(consulUri);
            }
        }

        public static void DeleteKeyValueElement(string key, Uri consulUri)
        {
            var consulClient = new ConsulClient(config => { config.Address = consulUri; });

            if (null == key)
            {
                throw new ArgumentException("Key cannot be null.");
            }

            try
            {
                if (null == consulClient.KV.Get(key).Result.Response)
                {
                    throw new ConsulKvStoreKeyNotFoundException(key);
                }

                var tmp = consulClient.KV.Delete(key).Result;
            }
            catch (ConsulKvStoreKeyNotFoundException)
            {
                throw;
            }
            catch (Exception)
            {
                throw new ConsulNotFoundException(consulUri);
            }
        }
    }
}