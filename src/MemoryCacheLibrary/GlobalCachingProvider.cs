using CacheStackEntity;
using System.Collections.Generic;

namespace MemoryCacheLayer
{
    public class GlobalCachingProvider : CachingProviderBase, IGlobalCachingProvider
    {
        private static readonly object _synclock = new object();
        private static GlobalCachingProvider _instance;

        private GlobalCachingProvider() { }

        /// <summary>
        /// Singleton GlobalCachingProvider
        /// </summary>
        public static IGlobalCachingProvider Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (_synclock)
                    {
                        if (_instance == null)
                        {
                            _instance = new GlobalCachingProvider();
                        }
                    }
                }
                return _instance;
            }
        }

        /// <summary>
        /// Add the object into MemoryCache
        /// </summary>
        /// <param name="persistenceCacheStackEntity"></param>
        /// <returns></returns>
        public new bool AddItem(PersistenceCacheStackEntity persistenceCacheStackEntity)
        {
            return base.AddItem(persistenceCacheStackEntity);
        }

        /// <summary>
        /// Add the items list into MemoryCache
        /// </summary>
        /// <param name="items"></param>
        /// <returns></returns>
        public new bool AddItems(List<PersistenceCacheStackEntity> items)
        {
            return base.AddItems(items);
        }

        /// <summary>
        /// Remove the item from the MemoryCache
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public new bool RemoveItem(string key)
        {
            return base.RemoveItem(key);
        }

        /// <summary>
        /// Get the item from the MemoryCache.
        /// Pass the FlagRemove = True to get and remove
        /// </summary>
        /// <param name="key"></param>
        /// <param name="flagRemove"></param>
        /// <returns></returns>
        public new PersistenceCacheStackEntity GetItem(string key, bool flagRemove)
        {
            return base.GetItem(key, flagRemove);
        }

        /// <summary>
        /// Get the items from the MemoryCache.
        /// Pass the FlagRemove = True to get and remove
        /// </summary>
        /// <param name="keys"></param>
        /// <param name="flagRemove"></param>
        /// <returns></returns>
        public new List<PersistenceCacheStackEntity> GetItems(List<string> keys, bool flagRemove)
        {
            return base.GetItems(keys, flagRemove);
        }

        /// <summary>
        /// Get all the items from MemoryCache
        /// </summary>
        /// <returns></returns>
        public new List<PersistenceCacheStackEntity> GetAllItem()
        {
            return base.GetAllItem();
        }

        /// <summary>
        /// Clear the current instance of MemoryCache
        /// </summary>
        /// <returns></returns>
        public new bool ClearCache()
        {
            return base.ClearCache();
        }
    }
}
