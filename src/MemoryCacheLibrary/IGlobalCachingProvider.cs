using CacheStackEntity;
using System.Collections.Generic;

namespace MemoryCacheLayer
{
    /// <summary>
    /// Interface for the GlobalCachingProvider
    /// </summary>
    public interface IGlobalCachingProvider
    {
        /// <summary>
        /// Add the object into MemoryCache
        /// </summary>
        /// <param name="persistenceCacheStackEntity"></param>
        /// <returns></returns>
        bool AddItem(PersistenceCacheStackEntity persistenceCacheStackEntity);

        /// <summary>
        /// Add the items list into MemoryCache
        /// </summary>
        /// <param name="items"></param>
        /// <returns></returns>
        bool AddItems(List<PersistenceCacheStackEntity> items);

        /// <summary>
        /// Remove the item from the MemoryCache
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        bool RemoveItem(string key);

        /// <summary>
        /// Get the item from the MemoryCache.
        /// Pass the FlagRemove = True to get and remove
        /// </summary>
        /// <param name="key"></param>
        /// <param name="flagRemove"></param>
        /// <returns></returns>
        PersistenceCacheStackEntity GetItem(string key, bool flagRemove);

        /// <summary>
        /// Get the items from the MemoryCache.
        /// Pass the FlagRemove = True to get and remove
        /// </summary>
        /// <param name="keys"></param>
        /// <param name="flagRemove"></param>
        /// <returns></returns>
        List<PersistenceCacheStackEntity> GetItems(List<string> keys, bool flagRemove);

        /// <summary>
        /// Get all the items from MemoryCache
        /// </summary>
        /// <returns></returns>
        List<PersistenceCacheStackEntity> GetAllItem();

        /// <summary>
        /// Clear the current instance of MemoryCache
        /// </summary>
        /// <returns></returns>
        bool ClearCache();
    }
}