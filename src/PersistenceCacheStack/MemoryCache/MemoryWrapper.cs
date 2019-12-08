/*
    This file is part of PersistenceCacheStack.

    PersistenceCacheStack is free software: you can redistribute it and/or modify
    it under the terms of the GNU General Public License as published by
    the Free Software Foundation, either version 3 of the License, or
    (at your option) any later version.

    PersistenceCacheStack is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU General Public License for more details.

    You should have received a copy of the GNU General Public License
    along with PersistenceCacheStack.  If not, see <http://www.gnu.org/licenses/>.
*/

using Microsoft.Extensions.Caching.Memory;
using PersistenceCacheStack.Entities;
using PersistenceCacheStack.Interface;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PersistenceCacheStack.MemoryCache
{
    /// <summary>
    /// The 1-layer MemoryCache wrapper
    /// </summary>
    public class MemoryWrapper : IMemoryCacheOperations
    {
        /// <summary>
        /// Return the StackEntity from MemoryCache
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public StackEntity Get(string key)
        {
            return (StackEntity) MemoryAccess.Instance.MemoryCache.Get(key);
        }

        /// <summary>
        /// Check if the key exists
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public bool Exists(string key)
        {
            return MemoryAccess.Instance.MemoryCache.Get(key) != null;
        }

        /// <summary>
        /// Insert the StackEntity object into MemoryCache
        /// </summary>
        /// <param name="stackEntity"></param>
        /// <returns></returns>
        public bool Insert(StackEntity stackEntity)
        {
            if(stackEntity.Expiration.HasValue)
                return MemoryAccess.Instance.MemoryCache.Set(stackEntity.Key, stackEntity, stackEntity.Expiration.Value) != null;
            return MemoryAccess.Instance.MemoryCache.Set(stackEntity.Key, stackEntity) != null;
        }

        /// <summary>
        /// Insert multiple StackEntity objects into MemoryCache
        /// </summary>
        /// <param name="stackEntityList"></param>
        public void Insert(List<StackEntity> stackEntityList)
        {
            if (stackEntityList != null)
            {
                Parallel.ForEach(stackEntityList, stackEntity =>
                {
                    if (stackEntity.Expiration.HasValue)
                    {
                        MemoryAccess.Instance.MemoryCache.Set(stackEntity.Key, stackEntity,
                            stackEntity.Expiration.Value);
                    }
                    else
                    {
                        MemoryAccess.Instance.MemoryCache.Set(stackEntity.Key, stackEntity);
                    }
                });
            }
        }

        /// <summary>
        /// Update the StackEntity object into MemoryCache
        /// </summary>
        /// <param name="stackEntity"></param>
        /// <returns></returns>
        public bool Update(StackEntity stackEntity)
        {
            return Insert(stackEntity);
        }

        /// <summary>
        /// Remove the StackEntity from MemoryCache
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public void Remove(string key)
        {
            MemoryAccess.Instance.MemoryCache.Remove(key);
        }

        /// <summary>
        /// Remove the StackEntity objects from MemoryCache
        /// </summary>
        /// <param name="keyList"></param>
        /// <returns></returns>
        public void Remove(List<string> keyList)
        {
            if (keyList != null)
            {
                Parallel.ForEach(keyList, key => { MemoryAccess.Instance.MemoryCache.Remove(key); });
            }
        }

        /// <summary>
        /// Clear the entire MemoryCache
        /// </summary>
        /// <returns></returns>
        public void Clear()
        {
            MemoryAccess.Instance.Reset();
        }
    }
}
