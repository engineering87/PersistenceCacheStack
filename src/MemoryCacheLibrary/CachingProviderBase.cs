using CacheStackEntity;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Caching;
using System.Threading.Tasks;

namespace MemoryCacheLayer
{
    public abstract class CachingProviderBase
    {
        protected MemoryCache cache = new MemoryCache("CachingProvider");

        static readonly object padlock = new object();

        public CachingProviderBase() { }

        /// <summary>
        /// Add the object into MemoryCache
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        protected virtual bool AddItem(PersistenceCacheStackEntity PersistenceCacheStackEntity)
        {
            try
            {
                var CacheItem = new CacheItem(PersistenceCacheStackEntity.Key, PersistenceCacheStackEntity);
                lock (padlock)
                {
                    if (PersistenceCacheStackEntity.Expiration == null)
                    {
                        cache.Set(CacheItem, new CacheItemPolicy());
                    }
                    else
                    {
                        // if it is required a cache expiration
                        cache.Set(CacheItem, new CacheItemPolicy()
                        {
                            AbsoluteExpiration = PersistenceCacheStackEntity.Expiration.Value
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
                    Parallel.ForEach(items, (PersistenceCacheStackEntity) =>
                    {
                        //var CacheItem = new CacheItem(PersistenceCacheStackEntity.Key, PersistenceCacheStackEntity.Object);
                        var CacheItem = new CacheItem(PersistenceCacheStackEntity.Key, PersistenceCacheStackEntity);
                        if (PersistenceCacheStackEntity.Expiration == null)
                        {
                            cache.Set(CacheItem, new CacheItemPolicy());
                        }
                        else
                        {
                            // if it is required a cache expiration
                            cache.Set(CacheItem, new CacheItemPolicy()
                            {
                                AbsoluteExpiration = PersistenceCacheStackEntity.Expiration.Value
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
                        if (remove == true)
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
                            if (remove == true)
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
                var CompleteList = new BlockingCollection<PersistenceCacheStackEntity>();
                lock (padlock)
                {
                    Parallel.ForEach(cache.Select(kvp => kvp.Value).ToList(), (Item) =>
                    {
                        CompleteList.Add((PersistenceCacheStackEntity)Item);
                    });
                }
                return CompleteList.ToList();
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
    }
}