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
        public DateTimeOffset? Expiration { get; set; }

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

        public override bool Equals(object obj)
        {
            if (obj == null)
                return false;

            PersistenceCacheStackEntity p = obj as PersistenceCacheStackEntity;
            if (p == null)
                return false;

            return Object.GetHashCode() == p.GetHashCode();
        }

        public override int GetHashCode()
        {
            return Key.GetHashCode() ^ Object.GetHashCode() ^ Expiration.GetHashCode();
        }

        public override string ToString()
        {
            if (Expiration != null)
            {
                return
                    $"{nameof(Key)}={Key} " +
                    $"{nameof(Object)}={this.Object.GetType().Name} " +
                    $"{nameof(Expiration)}={Expiration.Value}";
            }
            else
            {
                return 
                    $"{nameof(Key)}={Key} " +
                    $"{nameof(Object)}={this.Object.GetType().Name}";
            }
        }
    }
}