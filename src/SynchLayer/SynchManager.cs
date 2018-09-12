using MemoryCacheLayer;
using RedisLayer;
using System.Threading.Tasks;
using CacheStackEntity;

namespace PersistenceCacheStack
{
    public class SynchManager
    {
        private readonly TaskFactory taskFactory;
        private RedisWrapper redisWrapper;

        public SynchManager()
        {
            this.taskFactory = new TaskFactory(TaskCreationOptions.PreferFairness, TaskContinuationOptions.PreferFairness);
            this.redisWrapper = new RedisWrapper();
        }

        /// <summary>
        /// Synch all cached object from Redis to in-memory MemoryCache
        /// </summary>
        public void SynchFromRedis()
        {
            var objRedis = this.redisWrapper.GetAll();
            this.taskFactory.StartNew(() => GlobalCachingProvider.Instance.AddItems(objRedis));
        }

        /// <summary>
        /// Get the item from MemoryCache
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public PersistenceCacheStackEntity GetItem(string key)
        {
            var objCached = GlobalCachingProvider.Instance.GetItem(key, false);
            // fire and forget, check the status of the current key-object on Redis for any changes from other nodes
            this.taskFactory.StartNew(() => this.CheckExternalsUpdates(key, objCached));
            return objCached;
        }

        /// <summary>
        /// Add item into MemoryCache and Redis for persistence
        /// </summary>
        /// <param name="PersistenceCacheStackEntity"></param>
        /// <returns></returns>
        public bool AddItem(PersistenceCacheStackEntity PersistenceCacheStackEntity)
        {
            bool resultInMemory = GlobalCachingProvider.Instance.AddItem(PersistenceCacheStackEntity);
            if(true == resultInMemory)
            {
                this.taskFactory.StartNew(() => this.redisWrapper.Push(PersistenceCacheStackEntity));
                return resultInMemory;
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
            bool resultInMemory = GlobalCachingProvider.Instance.RemoveItem(key);
            if (true == resultInMemory)
            {
                this.taskFactory.StartNew(() => this.redisWrapper.Remove(key));
                return resultInMemory;
            }
            return false;
        }

        /// <summary>
        /// Update the PersistenceCacheStackEntity object into MemoryCache and Redis
        /// </summary>
        /// <param name="PersistenceCacheStackEntity"></param>
        /// <returns></returns>
        public bool UpdateItem(PersistenceCacheStackEntity PersistenceCacheStackEntity)
        {
            var objCached = GlobalCachingProvider.Instance.GetItem(PersistenceCacheStackEntity.Key, true);
            if (objCached != null)
            {
                var resultInMemory = GlobalCachingProvider.Instance.AddItem(PersistenceCacheStackEntity);
                if (true == resultInMemory)
                {
                    this.taskFactory.StartNew(() => this.redisWrapper.Update(PersistenceCacheStackEntity));
                }
                return resultInMemory;
            }
            return false;
        }

        /// <summary>
        /// Remove all the cached object from MemoryCache and from Redis
        /// </summary>
        public void ClearCache()
        {
            var clearResult = GlobalCachingProvider.Instance.ClearCache();
            if(true == clearResult)
            {
                this.taskFactory.StartNew(() => this.redisWrapper.Clear());
            }
        }

        /// <summary>
        /// Check the current state of the object into Redis and eventually sync the memory cache
        /// </summary>
        private void CheckExternalsUpdates(string key, PersistenceCacheStackEntity persistenceCacheStackEntity)
        {
            var objRedis = redisWrapper.Get(key);
            if(objRedis != null)
            {
                if(persistenceCacheStackEntity == null)
                {
                    // another node has added the object with key into redis, we must therefore add it into the MemoryCache
                    GlobalCachingProvider.Instance.AddItem(objRedis);
                }
                else if(!objRedis.Equals(persistenceCacheStackEntity))
                {
                    // another node has updated the object, we must therefore update it into the MemoryCache
                    GlobalCachingProvider.Instance.GetItem(key, true);
                    GlobalCachingProvider.Instance.AddItem(objRedis);
                }
            }
            else
            {
                // another node has deleted the key from redis, we must therefore remove it from the MemoryCache
                GlobalCachingProvider.Instance.GetItem(key, true);
            }
        }
    }
}
