using CacheStackEntity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RedisLayer
{
    /// <summary>
    /// Wrapper of StackExchange.Redis.Extensions
    /// </summary>
    public class RedisWrapper
    {
        /// <summary>
        /// Return the PersistenceCacheStackEntity from Redis
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public PersistenceCacheStackEntity Get(string key)
        {
            try
            {
                var obj = RedisAccess.Instance.RedisCacheClient.Get<PersistenceCacheStackEntity>(key);
                return (PersistenceCacheStackEntity)Convert.ChangeType(obj, typeof(PersistenceCacheStackEntity));
            }
            catch (Exception)
            {
                return default(PersistenceCacheStackEntity);
            }
        }

        /// <summary>
        /// Return a list of PersistenceCacheStackEntity from Redis
        /// </summary>
        /// <param name="keyList"></param>
        /// <returns></returns>
        public List<PersistenceCacheStackEntity> Get(List<string> keyList)
        {
            try
            {
                var obj = RedisAccess.Instance.RedisCacheClient.GetAll<PersistenceCacheStackEntity>(keyList);
                return (List<PersistenceCacheStackEntity>)Convert.ChangeType(obj, typeof(List<PersistenceCacheStackEntity>));
            }
            catch (Exception)
            {
                return default(List<PersistenceCacheStackEntity>);
            }
        }

        /// <summary>
        /// Get a list of all PersistenceCacheStackEntity from Redis
        /// </summary>
        /// <returns></returns>
        public List<PersistenceCacheStackEntity> GetAll()
        {
            try
            {
                var objList = new List<PersistenceCacheStackEntity>();
                var keys = RedisAccess.Instance.RedisCacheClient.SearchKeys("*");
                if (keys != null)
                {
                    Parallel.ForEach(keys, (key) =>
                    {
                        var obj = RedisAccess.Instance.RedisCacheClient.Get<PersistenceCacheStackEntity>(key);
                        if (obj != null)
                        {
                            objList.Add(obj);
                        }
                    });
                }
                return objList;
            }
            catch (Exception)
            {
                return default(List<PersistenceCacheStackEntity>);
            }
        }

        /// <summary>
        /// Insert the PersistenceCacheStackEntity object into Redis
        /// </summary>
        /// <param name="PersistenceCacheStackEntity"></param>
        /// <returns></returns>
        public bool Push(PersistenceCacheStackEntity pCacheStackEntity)
        {
            if (pCacheStackEntity.Expiration.HasValue)
            {
                return RedisAccess.Instance.RedisCacheClient.Add(pCacheStackEntity.Key, pCacheStackEntity, pCacheStackEntity.Expiration.Value);
            }
            return RedisAccess.Instance.RedisCacheClient.Add(pCacheStackEntity.Key, pCacheStackEntity);
        }

        /// <summary>
        /// Update the PersistenceCacheStackEntity object
        /// </summary>
        /// <param name="key"></param>
        /// <param name="expiresAt"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public bool Update(PersistenceCacheStackEntity pCacheStackEntity)
        {
            if (pCacheStackEntity.Expiration.HasValue)
            {
                return RedisAccess.Instance.RedisCacheClient.Replace(pCacheStackEntity.Key, pCacheStackEntity, pCacheStackEntity.Expiration.Value);
            }
            return RedisAccess.Instance.RedisCacheClient.Replace(pCacheStackEntity.Key, pCacheStackEntity);
        }

        /// <summary>
        /// Remove the PersistenceCacheStackEntity from Redis
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public bool Remove(string key)
        {
            return RedisAccess.Instance.RedisCacheClient.Remove(key);
        }

        /// <summary>
        /// Remove the PersistenceCacheStackEntity objects from Redis
        /// </summary>
        /// <param name="keyList"></param>
        /// <returns></returns>
        public bool Remove(List<string> keyList)
        {
            RedisAccess.Instance.RedisCacheClient.RemoveAll(keyList.AsEnumerable());
            return true;
        }

        /// <summary>
        /// Remove all the cached object from Redis
        /// </summary>
        /// <returns></returns>
        public bool Clear()
        {
            IEnumerable<string> keys = RedisAccess.Instance.RedisCacheClient.SearchKeys("*");
            if (keys != null)
            {
                return this.Remove(keys.ToList());
            }
            return false;
        }
    }
}
