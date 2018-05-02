using CacheStackEntity;
using MemoryCacheLayer;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using UnitTestProject.TestObject;

namespace UnitTestProject
{
    [TestClass]
    public class MemoryCacheTest
    {
        [TestMethod]
        public void TestAddItem()
        {
            UnitTestClass testClass = new UnitTestClass()
            {
                TestDescription = "test1",
                TestId = 1
            };

            PersistenceCacheStackEntity persistenceCacheStackEntity = new PersistenceCacheStackEntity("test1", testClass, null);

            var addResult = GlobalCachingProvider.Instance.AddItem(persistenceCacheStackEntity);
            Assert.IsTrue(addResult);
        }

        [TestMethod]
        public void TestGetItem()
        {
            UnitTestClass testClass = new UnitTestClass()
            {
                TestDescription = "test2",
                TestId = 2
            };

            PersistenceCacheStackEntity persistenceCacheStackEntity = new PersistenceCacheStackEntity("test2", testClass, null);

            var addResult = GlobalCachingProvider.Instance.AddItem(persistenceCacheStackEntity);
            Assert.IsTrue(addResult);

            var objectCached = GlobalCachingProvider.Instance.GetItem(persistenceCacheStackEntity.Key, false);
            Assert.IsNotNull(objectCached);
            Assert.AreEqual(objectCached.Key, "test2");
        }

        [TestMethod]
        public void TestRemoveItem()
        {
            UnitTestClass testClass = new UnitTestClass()
            {
                TestDescription = "test3",
                TestId = 3
            };

            PersistenceCacheStackEntity persistenceCacheStackEntity = new PersistenceCacheStackEntity("test3", testClass, null);

            var addResult = GlobalCachingProvider.Instance.AddItem(persistenceCacheStackEntity);
            Assert.IsTrue(addResult);

            var objectCached = GlobalCachingProvider.Instance.GetItem(persistenceCacheStackEntity.Key, true);
            Assert.IsNotNull(objectCached);

            objectCached = GlobalCachingProvider.Instance.GetItem(persistenceCacheStackEntity.Key, false);
            Assert.IsNull(objectCached);
        }

        [TestMethod]
        public void TestUpdateItem()
        {
            UnitTestClass testClass = new UnitTestClass()
            {
                TestDescription = "test4",
                TestId = 4
            };

            PersistenceCacheStackEntity persistenceCacheStackEntity = new PersistenceCacheStackEntity("test4", testClass, null);

            var addResult = GlobalCachingProvider.Instance.AddItem(persistenceCacheStackEntity);
            Assert.IsTrue(addResult);

            var objectCached = GlobalCachingProvider.Instance.GetItem(persistenceCacheStackEntity.Key, false);
            Assert.AreEqual(objectCached.Key, "test4");

            persistenceCacheStackEntity.Object = null;

            var updateResult = GlobalCachingProvider.Instance.AddItem(persistenceCacheStackEntity);
            Assert.IsTrue(updateResult);

            var updatedObject = GlobalCachingProvider.Instance.GetItem(persistenceCacheStackEntity.Key, false);
            Assert.IsNotNull(updatedObject);
            Assert.IsNull(updatedObject.Object);
        }
    }
}
