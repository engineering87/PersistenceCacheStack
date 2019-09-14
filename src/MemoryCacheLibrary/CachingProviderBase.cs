using CacheStackEntity;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Caching;
using System.Threading.Tasks;

namespace MemoryCacheLayer
{
    public abstract class CachingProviderBase : IDisposable
    {
        protected MemoryCache cache = new MemoryCache("CachingProvider");

        static readonly object padlock = new object();

        protected CachingProviderBase() { }

        /// <summary>
        /// Add the object into MemoryCache
        /// </summary>
        /// <param name="persistenceCacheStackEntity"></param>
        /// <returns></returns>
        protected virtual bool AddItem(PersistenceCacheStackEntity persistenceCacheStackEntity)
        {
            try
            {
                var cacheItem = new CacheItem(persistenceCacheStackEntity.Key, persistenceCacheStackEntity);
                lock (padlock)
                {
                    if (persistenceCacheStackEntity.Expiration == null)
                    {
                        cache.Set(cacheItem, new CacheItemPolicy());
                    }
                    else
                    {
                        // if it is required a cache expiration
                        cache.Set(cacheItem, new CacheItemPolicy()
                        {
                            AbsoluteExpiration = persistenceCacheStackEntity.Expiration.Value
                        });
                    }
                }
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        /// <summary>
        /// Massive add of items into MemoryCache
        /// </summary>
        /// <param name="items"></param>
        /// <returns></returns>
        protected virtual bool AddItems(List<PersistenceCacheStackEntity> items)
        {
            try
            {
                lock (padlock)
                {
                    Parallel.ForEach(items, (persistenceCacheStackEntity) =>
                    {
                        var cacheItem = new CacheItem(persistenceCacheStackEntity.Key, persistenceCacheStackEntity);
                        if (persistenceCacheStackEntity.Expiration == null)
                        {
                            cache.Set(cacheItem, new CacheItemPolicy());
                        }
                        else
                        {
                            // if it is required a cache expiration
                            cache.Set(cacheItem, new CacheItemPolicy()
                            {
                                AbsoluteExpiration = persistenceCacheStackEntity.Expiration.Value
                            });
                        }
                    });
                }
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        /// <summary>
        /// Remove the object related to the key
        /// </summary>
        /// <param name="key"></param>
        protected virtual bool RemoveItem(string key)
        {
            if (string.IsNullOrEmpty(key))
                return false;

            try
            {
                lock (padlock)
                {
                    var obj = cache.Remove(key);
                    if (obj != null)
                        return true;
                }
                return false;
            }
            catch (Exception)
            {
                return false;
            }
        }

        /// <summary>
        /// Return the object related to the key
        /// </summary>
        /// <param name="key"></param>
        /// <param name="remove"></param>
        /// <returns></returns>
        protected virtual PersistenceCacheStackEntity GetItem(string key, bool remove)
        {
            try
            {
                lock (padlock)
                {
                    var res = cache[key];
                    if (res != null)
                    {
                        if (remove)
                            cache.Remove(key);
                    }
                    return (PersistenceCacheStackEntity)res;
                }
            }
            catch (Exception)
            {
                return default(PersistenceCacheStackEntity);
            }
        }

        /// <summary>
        /// Return a list of objects related to the key list
        /// </summary>
        /// <param name="keys"></param>
        /// <param name="remove"></param>
        /// <returns></returns>
        protected virtual List<PersistenceCacheStackEntity> GetItems(List<string> keys, bool remove)
        {
            try
            {
                var items = new List<PersistenceCacheStackEntity>();
                lock (padlock)
                {
                    Parallel.ForEach(keys, (key) =>
                    {
                        var res = cache[key];
                        if (res != null)
                        {
                            items.Add((PersistenceCacheStackEntity)res);
                            if (remove)
                                cache.Remove(key);
                        }
                    });
                }
                return items;
            }
            catch (Exception)
            {
                return null;
            }
        }

        /// <summary>
        /// Return a list of all the objects
        /// </summary>
        /// <returns></returns>
        protected virtual List<PersistenceCacheStackEntity> GetAllItem()
        {
            try
            {
                var completeList = new BlockingCollection<PersistenceCacheStackEntity>();
                lock (padlock)
                {
                    Parallel.ForEach(cache.Select(kvp => kvp.Value).ToList(), (item) =>
                    {
                        completeList.Add((PersistenceCacheStackEntity)item);
                    });
                }
                return completeList.ToList();
            }
            catch (Exception)
            {
                return null;
            }
        }

        /// <summary>
        /// Remove all the current items
        /// </summary>
        protected virtual bool ClearCache()
        {
            try
            {
                var cache = MemoryCache.Default;
                lock (padlock)
                {
                    Parallel.ForEach(cache.Select(kvp => kvp.Key).ToList(), (cacheKey) =>
                    {
                        cache.Remove(cacheKey);
                    });
                }
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing && cache != null)
            {
                lock (padlock)
                {
                    cache.Dispose();
                    cache = null;
                }
            }
        }
    }
}