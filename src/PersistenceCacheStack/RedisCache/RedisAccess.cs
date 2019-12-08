/*
    This file is part of PersistenceCacheStack.

    PersistenceCacheStack is free software: you can redistribute it and/or modify
    it under the terms of the GNU General Public License as published by
    the Free Software Foundation, either version 3 of the License, or
    (at your option) any later version.

    PersistenceCacheStack is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU General Public License for more details.

    You should have received a copy of the GNU General Public License
    along with PersistenceCacheStack.  If not, see <http://www.gnu.org/licenses/>.
*/

using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Extensions.Configuration;
using StackExchange.Redis;

namespace PersistenceCacheStack.RedisCache
{
    internal class RedisAccess : IDisposable
    {
        private static readonly object _synclock = new object();
        private static RedisAccess _instance;

        /// <summary>
        /// The Redis connections Multiplexer
        /// </summary>
        private readonly Lazy<ConnectionMultiplexer> _connection;

        /// <summary>
        /// Get a Redis connection
        /// </summary>
        /// <returns></returns>
        public ConnectionMultiplexer GetConnection() => _connection.Value;

        /// <summary>
        /// Create a new instance of RedisClient
        /// </summary>
        private RedisAccess()
        {
            var config = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile(Config)
                .Build();

            var connectionString = config[RedisConnection];
            if (connectionString == null)
            {
                throw new KeyNotFoundException($"Environment variable for {RedisConnection} was not found.");
            }

            var conn = ConfigurationOptions.Parse(connectionString);
            conn.AllowAdmin = true;
            _connection = new Lazy<ConnectionMultiplexer>(() => ConnectionMultiplexer.Connect(conn));
        }

        /// <summary>
        /// Get the RedisClient instance
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

        private const string RedisConnection = "REDIS_CONNECTION";
        private const string Config = "config.json";

        public void Dispose()
        {
            if (_connection != null && _connection.IsValueCreated)
            {
                _connection.Value.Close();
                _connection.Value.Dispose();
            }
        }
    }
}
