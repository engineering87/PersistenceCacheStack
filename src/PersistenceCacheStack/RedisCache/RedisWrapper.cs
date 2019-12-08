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

using PersistenceCacheStack.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using PersistenceCacheStack.Interface;
using StackExchange.Redis;

namespace PersistenceCacheStack.RedisCache
{
    /// <summary>
    /// The 2-layer RedisCache wrapper.
    /// Redis connection failures are handled here.
    /// </summary>
    public class RedisWrapper : IRedisCacheOperations
    {
        /// <summary>
        /// Return the StackEntity from Redis
        /// </summary>
        /// <param name="key"></param>
        /// <param name="databaseNumber"></param>
        /// <returns></returns>
        public StackEntity Get(string key, int databaseNumber = 0)
        {
            try
            {
                var database = RedisAccess.Instance.GetConnection().GetDatabase(databaseNumber);

                var obj = database.StringGet(key);

                if (obj.HasValue)
                    return (StackEntity)Convert.ChangeType(obj, typeof(StackEntity));
                return default;
            }
            catch (RedisConnectionException)
            {
                return default;
            }
        }

        /// <summary>
        /// Return a StackEntity list from Redis
        /// </summary>
        /// <param name="databaseNumber"></param>
        /// <returns></returns>
        public List<StackEntity> GetAll(int databaseNumber = 0)
        {
            try
            {
                var entities = new List<StackEntity>();
                var database = RedisAccess.Instance.GetConnection().GetDatabase(databaseNumber);
                var endpoints = RedisAccess.Instance.GetConnection().GetEndPoints(true);

                foreach (var endpoint in endpoints)
                {
                    var server = RedisAccess.Instance.GetConnection().GetServer(endpoint);
                    var keys = server.Keys(databaseNumber);

                    entities.AddRange(keys.Select(
                        key => new StackEntity(key, database.StringGet(key)))
                    );
                }

                return entities;
            }
            catch (RedisConnectionException)
            {
                return default;
            }
        }

        /// <summary>
        /// Check if the key exists
        /// </summary>
        /// <param name="key"></param>
        /// <param name="databaseNumber"></param>
        /// <returns></returns>
        public bool Exists(string key, int databaseNumber = 0)
        {
            try
            {
                var database = RedisAccess.Instance.GetConnection().GetDatabase(databaseNumber);

                return database.KeyExists(key);
            }
            catch (RedisConnectionException)
            {
                return false;
            }
        }

        /// <summary>
        /// Insert the StackEntity object into Redis
        /// </summary>
        /// <param name="stackEntity"></param>
        /// <param name="databaseNumber"></param>
        /// <returns></returns>
        public bool Insert(StackEntity stackEntity, int databaseNumber = 0)
        {
            try
            {
                var database = RedisAccess.Instance.GetConnection().GetDatabase(databaseNumber);

                var redisEntity = new RedisEntity(stackEntity.Key, stackEntity);

                return stackEntity.Expiration != null
                    ? database.StringSet(stackEntity.Key, redisEntity.Value, stackEntity.Expiration.Value)
                    : database.StringSet(stackEntity.Key, redisEntity.Value);
            }
            catch (RedisConnectionException)
            {
                return false;
            }
        }

        /// <summary>
        /// Update the PersistenceCacheStackEntity object
        /// </summary>
        /// <param name="stackEntity"></param>
        /// <param name="databaseNumber"></param>
        /// <returns></returns>
        public bool Update(StackEntity stackEntity, int databaseNumber = 0)
        {
            try
            {
                var database = RedisAccess.Instance.GetConnection().GetDatabase(databaseNumber);

                var redisEntity = new RedisEntity(stackEntity.Key, stackEntity);

                return stackEntity.Expiration != null
                    ? database.StringSet(stackEntity.Key, redisEntity.Value, stackEntity.Expiration.Value)
                    : database.StringSet(stackEntity.Key, redisEntity.Value);
            }
            catch (RedisConnectionException)
            {
                return false;
            }
        }

        /// <summary>
        /// Remove the StackEntity from Redis
        /// </summary>
        /// <param name="key"></param>
        /// <param name="databaseNumber"></param>
        /// <returns></returns>
        public bool Remove(string key, int databaseNumber = 0)
        {
            try
            {
                var database = RedisAccess.Instance.GetConnection().GetDatabase(databaseNumber);

                return database.KeyDelete(key);
            }
            catch (RedisConnectionException)
            {
                return false;
            }
        }

        /// <summary>
        /// Remove the StackEntity objects from Redis
        /// </summary>
        /// <param name="keyList"></param>
        /// <param name="databaseNumber"></param>
        /// <returns></returns>
        public bool Remove(List<string> keyList, int databaseNumber = 0)
        {
            try
            {
                var database = RedisAccess.Instance.GetConnection().GetDatabase(databaseNumber);

                var result = true;
                foreach (var key in keyList)
                {
                    var currResult = database.KeyDelete(key);
                    if (!currResult)
                        result = false;
                }

                return result;
            }
            catch (RedisConnectionException)
            {
                return false;
            }
        }

        /// <summary>
        /// Clear all keys from default Redis database
        /// </summary>
        /// <returns></returns>
        public void Clear()
        {
            try
            {
                var endpoints = RedisAccess.Instance.GetConnection().GetEndPoints(true);
                foreach (var endpoint in endpoints)
                {
                    var server = RedisAccess.Instance.GetConnection().GetServer(endpoint);
                    server.FlushDatabase(); // default 0
                }
            }
            catch (RedisConnectionException)
            {
                // ignore
                // TODO retry
            }
        }

        /// <summary>
        /// Clear all keys from specified Redis database
        /// </summary>
        /// <param name="databaseNumber"></param>
        /// <returns></returns>
        public void Clear(int databaseNumber)
        {
            try
            {
                var endpoints = RedisAccess.Instance.GetConnection().GetEndPoints(true);
                foreach (var endpoint in endpoints)
                {
                    var server = RedisAccess.Instance.GetConnection().GetServer(endpoint);
                    server.FlushDatabase(databaseNumber);
                }
            }
            catch (RedisConnectionException)
            {
                // ignore
                // TODO retry
            }
        }

        /// <summary>
        /// Clear all keys from Redis
        /// </summary>
        public void ClearAll()
        {
            try
            {
                var endpoints = RedisAccess.Instance.GetConnection().GetEndPoints(true);
                foreach (var endpoint in endpoints)
                {
                    var server = RedisAccess.Instance.GetConnection().GetServer(endpoint);
                    server.FlushAllDatabases();
                }
            }
            catch (RedisConnectionException)
            {
                // ignore
                // TODO retry
            }
        }
    }
}
