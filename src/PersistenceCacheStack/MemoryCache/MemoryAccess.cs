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
using System.Threading;

namespace PersistenceCacheStack.MemoryCache
{
    internal class MemoryAccess
    {
        private static readonly object _synclock = new object();
        private static MemoryAccess _instance;

        /// <summary>
        /// The CancellationToken for MemoryCache reset
        /// </summary>
        private static CancellationTokenSource _resetCacheToken;

        /// <summary>
        /// The MemoryCache client interface
        /// </summary>
        public readonly IMemoryCache MemoryCache;

        private MemoryAccess()
        {
            var options = new MemoryCacheOptions();

            _resetCacheToken = new CancellationTokenSource();
            MemoryCache = new Microsoft.Extensions.Caching.Memory.MemoryCache(options);
        }

        /// <summary>
        /// Get the RedisClient instance
        /// </summary>
        public static MemoryAccess Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (_synclock)
                    {
                        if (_instance == null)
                        {
                            _instance = new MemoryAccess();
                        }
                    }
                }
                return _instance;
            }
        }

        /// <summary>
        /// Reset the MemoryCache
        /// </summary>
        public void Reset()
        {
            if (_resetCacheToken != null && !_resetCacheToken.IsCancellationRequested && _resetCacheToken.Token.CanBeCanceled)
            {
                _resetCacheToken.Cancel();
                _resetCacheToken.Dispose();
            }

            _resetCacheToken = new CancellationTokenSource();
        }
    }
}
