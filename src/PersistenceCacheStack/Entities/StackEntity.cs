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

using Newtonsoft.Json;
using System;

namespace PersistenceCacheStack.Entities
{
    /// <summary>
    /// This class represent the main object into CacheStack environment
    /// </summary>
    [Serializable]
    public class StackEntity
    {
        public string Key { get; }
        public object Payload { get; }
        public TimeSpan? Expiration { get; }
        
        /// <summary>
        /// Constructor with key expiration
        /// </summary>
        /// <param name="key"></param>
        /// <param name="payload"></param>
        /// <param name="expiration"></param>
        public StackEntity(string key, object payload, TimeSpan? expiration)
        {
            if (string.IsNullOrEmpty(key) || payload == null)
                throw new ArgumentException("Key or Payload cannot be null");

            var type = payload.GetType();
            if (type.IsSerializable)
            {
                Key = key;
                Payload = payload;
                Expiration = expiration;
            }
            else
            {
                throw new ArgumentException("Payload cannot be serialized");
            }
        }

        /// <summary>
        /// Constructor with no key expiration
        /// </summary>
        /// <param name="key"></param>
        /// <param name="payload"></param>
        public StackEntity(string key, object payload)
        {
            if (string.IsNullOrEmpty(key) || payload == null)
                throw new ArgumentException("Key or Payload cannot be null");

            var type = payload.GetType();
            if (type.IsSerializable)
            {
                Key = key;
                Payload = payload;
                Expiration = null;
            }
            else
            {
                throw new ArgumentException("Payload cannot be serialized");
            }
        }

        /// <summary>
        /// Equals override method
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override bool Equals(object obj)
        {
            if (obj == null || !(obj is StackEntity p))
                return false;

            return Payload.GetHashCode() == p.GetHashCode();
        }

        /// <summary>
        /// Hashcode override method
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode()
        {
            return Key.GetHashCode() ^ Payload.GetHashCode() ^ Expiration.GetHashCode();
        }

        /// <summary>
        /// ToString override method
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return JsonConvert.SerializeObject(this);
        }
    }
}
