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
using Newtonsoft.Json;
using StackExchange.Redis;

namespace PersistenceCacheStack.Entities
{
    /// <summary>
    /// This class represent the Redis object that incapsulate the CacheStackEntity
    /// </summary>
    [Serializable]
    public class RedisEntity
    {
        public RedisKey Key { get; }
        public RedisValue Value { get; }

        /// <summary>
        /// The Redis database destination number
        /// </summary>
        public int DatabaseNumber { get; }

        /// <summary>
        /// Create a new instance of the RedisEntity
        /// </summary>
        /// <param name="key"></param>
        /// <param name="stackEntity"></param>
        public RedisEntity(string key, StackEntity stackEntity)
        {
            Key = key;
            Value = JsonConvert.SerializeObject(stackEntity); // serialize the payload into the RedisValue
            DatabaseNumber = 0; // default Redis database
        }

        /// <summary>
        /// Create a new instance of the RedisEntity
        /// </summary>
        /// <param name="key"></param>
        /// <param name="stackEntity"></param>
        /// <param name="databaseNumber"></param>
        public RedisEntity(string key, StackEntity stackEntity, int databaseNumber)
        {
            Key = key;
            Value = JsonConvert.SerializeObject(stackEntity); // serialize the payload into the RedisValue
            DatabaseNumber = databaseNumber;
        }

        /// <summary>
        /// Return the PersistenceCacheStackEntity object from the RedisValue
        /// </summary>
        /// <returns></returns>
        public StackEntity GetStackEntity()
        {
            return Value.HasValue ? JsonConvert.DeserializeObject<StackEntity>(Value) : default;
        }
    }
}
