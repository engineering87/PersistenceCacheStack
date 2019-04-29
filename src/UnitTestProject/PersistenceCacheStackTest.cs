using CacheStackEntity;
using PersistenceCacheStack;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RedisLayer;
using UnitTestProject.TestObject;

namespace UnitTestProject
{
    [TestClass]
    public class PersistenceCacheStackTest
    {
        [TestMethod]
        public void TestAddItem()
        {
            var testClass = new UnitTestClass()
            {
                TestDescription = "TestAddItem",
                TestId = 1
            };

            var persistenceCacheStack = new PersistenceCacheStackClient<UnitTestClass>(true);

            var addResult = persistenceCacheStack.AddItem("TestAddItem", testClass, null);
            Assert.IsTrue(addResult);
        }

        [TestMethod]
        public void TestGetItem()
        {
            var testClass = new UnitTestClass()
            {
                TestDescription = "TestAddItem",
                TestId = 2
            };

            var persistenceCacheStack = new PersistenceCacheStackClient<UnitTestClass>(true);

            var addResult = persistenceCacheStack.AddItem("TestAddItem", testClass, null);
            Assert.IsTrue(addResult);

            var objectCached = persistenceCacheStack.GetItem("TestAddItem");
            Assert.IsNotNull(objectCached);
            Assert.IsTrue(objectCached.TestId == 2);
        }

        [TestMethod]
        public void TestRemoveItem()
        {
            var testClass = new UnitTestClass()
            {
                TestDescription = "TestRemoveItem",
                TestId = 3
            };

            var persistenceCacheStack = new PersistenceCacheStackClient<UnitTestClass>(true);

            var addResult = persistenceCacheStack.AddItem("TestRemoveItem", testClass, null);
            Assert.IsTrue(addResult);

            var removeResult = persistenceCacheStack.RemoveItem("TestRemoveItem");
            Assert.IsTrue(removeResult);

            var cachedObject = persistenceCacheStack.GetItem("TestRemoveItem");
            var cachedObjectRedis = new RedisWrapper().Get("TestRemoveItem");
            Assert.IsNull(cachedObject);
            Assert.IsNull(cachedObjectRedis);
        }

        [TestMethod]
        public void TestSynchFromRedis()
        {
            var testClass = new UnitTestClass()
            {
                TestDescription = "TestSynchFromRedis",
                TestId = 4
            };

            var persistenceCacheStackEntity = new PersistenceCacheStackEntity("TestSynchFromRedis", testClass, null);

            var redisWrapper = new RedisWrapper();

            // add fake object into Redis
            var addResult = redisWrapper.Push(persistenceCacheStackEntity);
            Assert.IsTrue(addResult);

            var objectCached = redisWrapper.Get(persistenceCacheStackEntity.Key);
            Assert.IsNotNull(objectCached);

            // synch from Redis
            var persistenceCacheStack = new PersistenceCacheStackClient<UnitTestClass>(true);

            var obj = persistenceCacheStack.GetItem("TestSynchFromRedis");
            Assert.IsNotNull(obj);
            Assert.IsTrue(obj.TestId == 4);
        }
    }
}
