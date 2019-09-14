using CacheStackEntity;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RedisLayer;

namespace UnitTestProject
{
    [TestClass]
    public class RedisTest
    {
        [TestMethod]
        public void TestAddItem()
        {
            var persistenceCacheStackEntity = new PersistenceCacheStackEntity("test", new object(), null);
            var addResult = new RedisWrapper().Push(persistenceCacheStackEntity);
            Assert.IsTrue(addResult);
        }

        [TestMethod]
        public void TestGetItem()
        {
            var persistenceCacheStackEntity = new PersistenceCacheStackEntity("test", new object(), null);

            var redisWrapper = new RedisWrapper();

            var addResult = redisWrapper.Push(persistenceCacheStackEntity);
            Assert.IsTrue(addResult);

            var objectCached = redisWrapper.Get(persistenceCacheStackEntity.Key);
            Assert.IsNotNull(objectCached);
        }

        [TestMethod]
        public void TestRemoveItem()
        {
            var persistenceCacheStackEntity = new PersistenceCacheStackEntity("test", new object(), null);

            var redisWrapper = new RedisWrapper();

            var addResult = redisWrapper.Push(persistenceCacheStackEntity);
            Assert.IsTrue(addResult);

            var objectCached = redisWrapper.Get(persistenceCacheStackEntity.Key);
            Assert.IsNotNull(objectCached);

            var removeResult = redisWrapper.Remove(persistenceCacheStackEntity.Key);
            Assert.IsTrue(removeResult);

            objectCached = redisWrapper.Get(persistenceCacheStackEntity.Key);
            Assert.IsNull(objectCached);
        }
    }
}
