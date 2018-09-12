using System;

namespace CacheStackEntity
{
    /// <summary>
    /// This class represent the main object into CacheStack environment
    /// </summary>
    [Serializable]
    public class PersistenceCacheStackEntity
    {
        public string Key { get; private set; }
        public object Object { get; set; }
        public DateTimeOffset? Expiration { get; private set; }

        /// <summary>
        /// Constructor with key expiration
        /// </summary>
        /// <param name="Key"></param>
        /// <param name="Object"></param>
        /// <param name="Expiration"></param>
        public PersistenceCacheStackEntity(string Key, object Object, DateTimeOffset? Expiration)
        {
            if(string.IsNullOrEmpty(Key) || Object == null)
                throw new ArgumentException("Key or Object cannot be null");

            Type type = Object.GetType();
            if (true == type.IsSerializable)
            {
                this.Key = Key;
                this.Object = Object;
                this.Expiration = Expiration;
            }
            else
            {
                throw new ArgumentException("Object cannot be serialize");
            }
        }

        /// <summary>
        /// Constructor with no key expiration
        /// </summary>
        /// <param name="Key"></param>
        /// <param name="Object"></param>
        public PersistenceCacheStackEntity(string Key, object Object)
        {
            if (string.IsNullOrEmpty(Key) || Object == null)
                throw new ArgumentException("Key or Object cannot be null");

            Type type = Object.GetType();
            if (true == type.IsSerializable)
            {
                this.Key = Key;
                this.Object = Object;
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
            else
            {
                return 
                    $"{nameof(Key)}={Key} " +
                    $"{nameof(Object)}={Object.GetType().Name}";
            }
        }
    }
}