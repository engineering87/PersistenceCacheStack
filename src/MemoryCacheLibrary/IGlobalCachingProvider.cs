using CacheStackEntity;
using System.Collections.Generic;

namespace MemoryCacheLayer
{
    /// <summary>
    /// Interface for the GlobalCachingProvider
    /// </summary>
    public interface IGlobalCachingProvider
    {
        bool AddItem(PersistenceCacheStackEntity PersistenceCacheStackEntity);
        bool AddItems(List<PersistenceCacheStackEntity> items);
        bool RemoveItem(string key);
        PersistenceCacheStackEntity GetItem(string key, bool flagRemove);
        List<PersistenceCacheStackEntity> GetItems(List<string> keys, bool flagRemove);
        List<PersistenceCacheStackEntity> GetAllItem();
        bool ClearCache();
    }
}