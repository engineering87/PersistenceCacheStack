using CacheStackEntity;
using System;

namespace PersistenceCacheStack
{
    public class PersistenceCacheStackClient<T>
    {
        private SynchManager SynchManager;

        public PersistenceCacheStackClient()
        {
            this.SynchManager = new SynchManager();
            this.SynchManager.SynchFromRedis();
        }

        /// <summary>
        /// Get the object from MemoryCache using generics
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public T GetItem(string key)
        {
            var obj = this.SynchManager.GetItem(key);
            if (obj != null && obj.Object is T)
            {
                return (T)obj.Object;
            }
            return default(T);
        }

        /// <summary>
        /// Insert the object into MemoryCache and synch with Redis for persistence
        /// </summary>
        /// <param name="key"></param>
        /// <param name="obj"></param>
        /// <param name="Expiration"></param>
        /// <returns></returns>
        public bool AddItem(string key, T obj, DateTimeOffset? Expiration)
        {
            var PersistenceCacheStackEntity = new PersistenceCacheStackEntity(key, obj, Expiration);
            return this.SynchManager.AddItem(PersistenceCacheStackEntity);
        }

        /// <summary>
        /// Remove the object with Key = key from MemoryCache and from Redis
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public bool RemoveItem(string key)
        {
            return this.SynchManager.RemoveItem(key);
        }

        /// <summary>
        /// Update the object with Key = key into MemoryCache and Redis
        /// </summary>
        /// <param name="key"></param>
        /// <param name="obj"></param>
        /// <param name="Expiration"></param>
        /// <returns></returns>
        public bool UpdateItem(string key, T obj, DateTimeOffset? Expiration)
        {
            var PersistenceCacheStackEntity = new PersistenceCacheStackEntity(key, obj, Expiration);
            return this.SynchManager.UpdateItem(PersistenceCacheStackEntity);
        }

        /// <summary>
        /// Explicit synch all cached object from Redis to MemoryCache
        /// </summary>
        public void SynchFromRedis()
        {
            this.SynchManager.SynchFromRedis();
        }

        /// <summary>
        /// Clear all cached object from MemoryCache and from Redis
        /// </summary>
        public void Clear()
        {
            this.SynchManager.ClearCache();
        }
    }
}
