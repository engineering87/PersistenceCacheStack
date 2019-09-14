using System;

namespace CacheStackEntity
{
    /// <summary>
    /// This class represent the main object into CacheStack environment
    /// </summary>
    [Serializable]
    public class PersistenceCacheStackEntity
    {
        public string Key { get; }
        public object Object { get; }
        public DateTimeOffset? Expiration { get; }

        /// <summary>
        /// Constructor with key expiration
        /// </summary>
        /// <param name="key"></param>
        /// <param name="obj"></param>
        /// <param name="expiration"></param>
        public PersistenceCacheStackEntity(string key, object obj, DateTimeOffset? expiration)
        {
            if(string.IsNullOrEmpty(key) || obj == null)
                throw new ArgumentException("Key or Object cannot be null");

            var type = obj.GetType();
            if (true == type.IsSerializable)
            {
                this.Key = key;
                this.Object = obj;
                this.Expiration = expiration;
            }
            else
            {
                throw new ArgumentException("Object cannot be serialize");
            }
        }

        /// <summary>
        /// Constructor with no key expiration
        /// </summary>
        /// <param name="key"></param>
        /// <param name="obj"></param>
        public PersistenceCacheStackEntity(string key, object obj)
        {
            if (string.IsNullOrEmpty(key) || obj == null)
                throw new ArgumentException("Key or Object cannot be null");

            var type = obj.GetType();
            if (true == type.IsSerializable)
            {
                this.Key = key;
                this.Object = obj;
                this.Expiration = null;
            }
            else
            {
                throw new ArgumentException("Object cannot be serialize");
            }
        }

        /// <summary>
        /// Equals override method
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override bool Equals(object obj)
        {
            if (obj == null || !(obj is PersistenceCacheStackEntity p))
                return false;

            return Object.GetHashCode() == p.GetHashCode();
        }

        /// <summary>
        /// Hashcode override method
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode()
        {
            return Key.GetHashCode() ^ Object.GetHashCode() ^ Expiration.GetHashCode();
        }

        /// <summary>
        /// ToString override method
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            if (Expiration != null)
            {
                return
                    $"{nameof(Key)}={Key} " +
                    $"{nameof(Object)}={Object.GetType().Name} " +
                    $"{nameof(Expiration)}={Expiration.Value}";
            }

            return 
                $"{nameof(Key)}={Key} " +
                $"{nameof(Object)}={Object.GetType().Name}";
        }
    }
}