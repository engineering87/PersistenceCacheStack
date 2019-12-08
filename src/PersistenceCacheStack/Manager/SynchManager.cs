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
using PersistenceCacheStack.MemoryCache;
using PersistenceCacheStack.RedisCache;
using System.Threading.Tasks;

namespace PersistenceCacheStack.Manager
{
    /// <summary>
    /// This class deals with all the synch activity through in-memory and Redis layers
    /// </summary>
    public class SynchManager
    {
        private readonly TaskFactory _taskFactory;
        private readonly RedisWrapper _redisWrapper;
        private readonly MemoryWrapper _memoryWrapper;

        public SynchManager()
        {
            _taskFactory = new TaskFactory(TaskCreationOptions.PreferFairness, TaskContinuationOptions.PreferFairness);
            _redisWrapper = new RedisWrapper();
            _memoryWrapper = new MemoryWrapper();
        }

        /// <summary>
        /// Synch all cached object from Redis to in-memory MemoryCache
        /// </summary>
        public void SynchFromRedis()
        {
            var objRedis = _redisWrapper.GetAll();
            if (objRedis != null && objRedis.Count > 0)
            {
                _taskFactory.StartNew(() => _memoryWrapper.Insert(objRedis));
            }
        }

        /// <summary>
        /// Get the item from MemoryCache
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public StackEntity GetItem(string key)
        {
            var objCached = _memoryWrapper.Get(key);
            if (objCached != null)
            {
                // fire and forget, check the status of the current key-object on Redis for any changes from other nodes
                _taskFactory.StartNew(() => CheckExternalsUpdates(key, objCached));
            }
            return objCached;
        }

        /// <summary>
        /// Get and remove the item from MemoryCache and Redis
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public StackEntity GetRemoveItem(string key)
        {
            var objCached = _memoryWrapper.Get(key);
            if (objCached != null)
            {
                _memoryWrapper.Remove(key);
                _taskFactory.StartNew(() => _redisWrapper.Remove(key));
            }
            return objCached;
        }

        /// <summary>
        /// Check if the key exist
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public bool ExistItem(string key)
        {
            return GetItem(key) != null;
        }

        /// <summary>
        /// Add item into MemoryCache and Redis for persistence
        /// </summary>
        /// <param name="stackEntity"></param>
        /// <returns></returns>
        public bool AddItem(StackEntity stackEntity)
        {
            var resultInMemory = _memoryWrapper.Insert(stackEntity);
            if (resultInMemory)
            {
                _taskFactory.StartNew(() => _redisWrapper.Insert(stackEntity));
                return true;
            }
            return false;
        }

        /// <summary>
        /// Remove item from MemoryCache and from Redis
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public bool RemoveItem(string key)
        {
            _memoryWrapper.Remove(key);
            _taskFactory.StartNew(() => _redisWrapper.Remove(key));
            return true;
        }

        /// <summary>
        /// Update the StackEntity object into MemoryCache and Redis
        /// </summary>
        /// <param name="stackEntity"></param>
        /// <returns></returns>
        public bool UpdateItem(StackEntity stackEntity)
        {
            var resultInMemory = _memoryWrapper.Update(stackEntity);
            if (resultInMemory)
            {
                _taskFactory.StartNew(() => _redisWrapper.Update(stackEntity));
            }
            return resultInMemory;
        }

        /// <summary>
        /// Remove all the cached object from MemoryCache and from Redis
        /// </summary>
        public void ClearCache()
        {
            _memoryWrapper.Clear();
            _taskFactory.StartNew(() => _redisWrapper.Clear());
        }

        /// <summary>
        /// Check the current state of the object into Redis and eventually sync the memory cache
        /// </summary>
        private void CheckExternalsUpdates(string key, StackEntity stackEntity)
        {
            var objRedis = _redisWrapper.Get(key);
            if (objRedis != null)
            {
                if (stackEntity == null)
                {
                    // another node has added the object with key into redis, we must therefore add it into the MemoryCache
                    _memoryWrapper.Insert(objRedis);
                }
                else if (!objRedis.Equals(stackEntity.Payload) || objRedis.Expiration != stackEntity.Expiration)
                {
                    // another node has updated the object (payload or expiration), we must therefore update it into the MemoryCache
                    _memoryWrapper.Update(stackEntity);
                }
            }
            else
            {
                // another node has deleted the key from Redis, we must therefore remove it from the in-memory layer
                _memoryWrapper.Remove(key);
            }
        }
    }
}
