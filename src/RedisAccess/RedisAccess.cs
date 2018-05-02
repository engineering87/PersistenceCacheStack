﻿using StackExchange.Redis.Extensions.Core;
using StackExchange.Redis.Extensions.LegacyConfiguration;
using StackExchange.Redis.Extensions.Newtonsoft;

namespace RedisLayer
{
    public class RedisAccess
    {
        private static object _synclock = new object();
        private static RedisAccess _instance;
        private NewtonsoftSerializer _serializer;
        public StackExchangeRedisCacheClient RedisCacheClient { get; private set; }

        private RedisAccess()
        {
            _serializer = new NewtonsoftSerializer();
            var RedisConfiguration = RedisCachingSectionHandler.GetConfig();
            RedisCacheClient = new StackExchangeRedisCacheClient(_serializer, RedisConfiguration);
        }

        /// <summary>
        /// Singleton RedisAccess
        /// </summary>
        public static RedisAccess Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (_synclock)
                    {
                        if (_instance == null)
                        {
                            _instance = new RedisAccess();
                        }
                    }
                }
                return _instance;
            }
        }
    }
}
