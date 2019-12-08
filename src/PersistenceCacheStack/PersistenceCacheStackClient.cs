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
using PersistenceCacheStack.Entities;
using PersistenceCacheStack.Manager;

namespace PersistenceCacheStack
{
    public class PersistenceCacheStackClient<T>
    {
        /// <summary>
        /// The cache synchronization module
        /// </summary>
        private readonly SynchManager _synchManager;

        /// <summary>
        /// PersistenceCacheStack constructor
        /// </summary>
        public PersistenceCacheStackClient()
        {
            _synchManager = new SynchManager();
            _synchManager.SynchFromRedis();
        }

        /// <summary>
        /// PersistenceCacheStack constructor
        /// Take the SynchFromRedis flag for init synch from Redis
        /// </summary>
        /// <param name="synchFromRedis"></param>
        public PersistenceCacheStackClient(bool synchFromRedis)
        {
            _synchManager = new SynchManager();
            if (synchFromRedis)
            {
                _synchManager.SynchFromRedis();
            }
        }

        /// <summary>
        /// Get the key from the CacheStack
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public T GetItem(string key)
        {
            var obj = _synchManager.GetItem(key);
            if (obj?.Payload is T objObject)
            {
                return objObject;
            }
            return default;
        }

        /// <summary>
        /// Get and remove the key from the CacheStack
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public T GetRemoveItem(string key)
        {
            var obj = _synchManager.GetRemoveItem(key);
            if (obj?.Payload is T objObject)
            {
                return objObject;
            }
            return default;
        }

        /// <summary>
        /// Check if the key exist
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public bool ExistItem(string key)
        {
            return _synchManager.ExistItem(key);
        }

        /// <summary>
        /// Insert the object into MemoryCache and synch with Redis for persistence
        /// </summary>
        /// <param name="key"></param>
        /// <param name="obj"></param>
        /// <param name="expiration"></param>
        /// <returns></returns>
        public bool AddItem(string key, T obj, TimeSpan expiration)
        {
            var pCacheStackEntity = new StackEntity(key, obj, expiration);
            return _synchManager.AddItem(pCacheStackEntity);
        }

        /// <summary>
        /// Insert the object into MemoryCache and synch with Redis for persistence with no expiration
        /// </summary>
        /// <param name="key"></param>
        /// <param name="obj"></param>
        /// <returns></returns>
        public bool AddItem(string key, T obj)
        {
            var pCacheStackEntity = new StackEntity(key, obj, null);
            return _synchManager.AddItem(pCacheStackEntity);
        }

        /// <summary>
        /// Remove the key from MemoryCache and from Redis
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public bool RemoveItem(string key)
        {
            return _synchManager.RemoveItem(key);
        }

        /// <summary>
        /// Update the object with Key = key into MemoryCache and Redis
        /// </summary>
        /// <param name="key"></param>
        /// <param name="obj"></param>
        /// <param name="expiration"></param>
        /// <returns></returns>
        public bool UpdateItem(string key, T obj, TimeSpan? expiration)
        {
            var pCacheStackEntity = new StackEntity(key, obj, expiration);
            return _synchManager.UpdateItem(pCacheStackEntity);
        }

        /// <summary>
        /// Explicit synch all cached object from Redis to MemoryCache
        /// </summary>
        public void SynchFromRedis()
        {
            _synchManager.SynchFromRedis();
        }

        /// <summary>
        /// Clear all cached object from MemoryCache and from Redis
        /// </summary>
        public void Clear()
        {
            _synchManager.ClearCache();
        }
    }
}
