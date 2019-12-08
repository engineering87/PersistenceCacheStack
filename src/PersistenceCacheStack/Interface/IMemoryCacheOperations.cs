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
    /// 1-layer MemoryCache interface
    /// </summary>
    public interface IMemoryCacheOperations
    {
        StackEntity Get(string key);
        bool Exists(string key);
        bool Insert(StackEntity stackEntity);
        void Insert(List<StackEntity> stackEntity);
        bool Update(StackEntity stackEntity);
        void Remove(string key);
        void Remove(List<string> keyList);
        void Clear();
    }
}
