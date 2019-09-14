using CacheStackEntity;
using System;

namespace PersistenceCacheStack
{
    public class PersistenceCacheStackClient<T>
    {
        /// <summary>
        /// The cache synchronization module
        /// </summary>
        private readonly SynchManager synchManager;

        /// <summary>
        /// PersistenceCacheStack constructor
        /// </summary>
        public PersistenceCacheStackClient()
        {
            this.synchManager = new SynchManager();
            this.synchManager.SynchFromRedis();
        }

        /// <summary>
        /// PersistenceCacheStack constructor
        /// Take the SynchFromRedis flag for init synch from Redis
        /// </summary>
        /// <param name="synchFromRedis"></param>
        public PersistenceCacheStackClient(bool synchFromRedis)
        {
            this.synchManager = new SynchManager();
            if (synchFromRedis)
            {
                this.synchManager.SynchFromRedis();
            }
        }

        /// <summary>
        /// Get the object from MemoryCache using generics
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public T GetItem(string key)
        {
            var obj = this.synchManager.GetItem(key);
            if (obj?.Object is T objObject)
            {
                return objObject;
            }
            return default;
        }

        /// <summary>
        /// Insert the object into MemoryCache and synch with Redis for persistence
        /// </summary>
        /// <param name="key"></param>
        /// <param name="obj"></param>
        /// <param name="expiration"></param>
        /// <returns></returns>
        public bool AddItem(string key, T obj, DateTimeOffset? expiration)
        {
            var pCacheStackEntity = new PersistenceCacheStackEntity(key, obj, expiration);
            return this.synchManager.AddItem(pCacheStackEntity);
        }

        /// <summary>
        /// Remove the object with Key = key from MemoryCache and from Redis
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public bool RemoveItem(string key)
        {
            return this.synchManager.RemoveItem(key);
        }

        /// <summary>
        /// Update the object with Key = key into MemoryCache and Redis
        /// </summary>
        /// <param name="key"></param>
        /// <param name="obj"></param>
        /// <param name="expiration"></param>
        /// <returns></returns>
        public bool UpdateItem(string key, T obj, DateTimeOffset? expiration)
        {
            var pCacheStackEntity = new PersistenceCacheStackEntity(key, obj, expiration);
            return this.synchManager.UpdateItem(pCacheStackEntity);
        }

        /// <summary>
        /// Explicit synch all cached object from Redis to MemoryCache
        /// </summary>
        public void SynchFromRedis()
        {
            this.synchManager.SynchFromRedis();
        }

        /// <summary>
        /// Clear all cached object from MemoryCache and from Redis
        /// </summary>
        public void Clear()
        {
            this.synchManager.ClearCache();
        }
    }
}
