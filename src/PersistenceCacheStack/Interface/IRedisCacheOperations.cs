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

using PersistenceCacheStack.Entities;
using System.Collections.Generic;

namespace PersistenceCacheStack.Interface
{
    /// <summary>
    /// 2-layer RedisCache interface
    /// </summary>
    public interface IRedisCacheOperations
    {
        StackEntity Get(string key, int databaseNumber = 0);
        List<StackEntity> GetAll(int databaseNumber = 0);
        bool Exists(string key, int databaseNumber = 0);
        bool Insert(StackEntity pCacheStackEntity, int databaseNumber = 0);
        bool Update(StackEntity pCacheStackEntity, int databaseNumber = 0);
        bool Remove(string key, int databaseNumber = 0);
        bool Remove(List<string> keyList, int databaseNumber = 0);
        void Clear();
        void Clear(int databaseNumber);
        void ClearAll();
    }
}
