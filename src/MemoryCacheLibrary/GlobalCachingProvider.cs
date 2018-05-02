using CacheStackEntity;
using System;
using System.Collections.Generic;

namespace MemoryCacheLayer
{
    public class GlobalCachingProvider : CachingProviderBase, IGlobalCachingProvider
    {
        private static object _synclock = new object();
        private static GlobalCachingProvider _instance;
        private GlobalCachingProvider() { }

        /// <summary>
        /// Singleton GlobalCachingProvider
        /// </summary>
        public static GlobalCachingProvider Instance
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

        public virtual new bool AddItem(PersistenceCacheStackEntity PersistenceCacheStackEntity)
        {
            return base.AddItem(PersistenceCacheStackEntity);
        }

        public virtual new bool AddItems(List<PersistenceCacheStackEntity> items)
        {
            return base.AddItems(items);
        }

        public virtual new bool RemoveItem(string key)
        {
            return base.RemoveItem(key);
        }

        public virtual new PersistenceCacheStackEntity GetItem(string key, bool flagRemove)
        {
            return base.GetItem(key, flagRemove);
        }

        public virtual new List<PersistenceCacheStackEntity> GetItems(List<string> keys, bool flagRemove)
        {
            return base.GetItems(keys, flagRemove);
        }

        public virtual new List<PersistenceCacheStackEntity> GetAllItem()
        {
            return base.GetAllItem();
        }

        public virtual new bool ClearCache()
        {
            return base.ClearCache();
        }
    }
}
