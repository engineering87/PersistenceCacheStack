﻿using CacheStackEntity;
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
            var testClass = new UnitTestClass()
            {
                TestDescription = "test1",
                TestId = 1
            };

            var persistenceCacheStackEntity = new PersistenceCacheStackEntity("test1", testClass, null);

            var addResult = GlobalCachingProvider.Instance.AddItem(persistenceCacheStackEntity);
            Assert.IsTrue(addResult);
        }

        [TestMethod]
        public void TestGetItem()
        {
            var testClass = new UnitTestClass()
            {
                TestDescription = "test2",
                TestId = 2
            };

            var persistenceCacheStackEntity = new PersistenceCacheStackEntity("test2", testClass, null);

            var addResult = GlobalCachingProvider.Instance.AddItem(persistenceCacheStackEntity);
            Assert.IsTrue(addResult);

            var objectCached = GlobalCachingProvider.Instance.GetItem(persistenceCacheStackEntity.Key, false);
            Assert.IsNotNull(objectCached);
            Assert.AreEqual(objectCached.Key, "test2");
        }

        [TestMethod]
        public void TestRemoveItem()
        {
            var testClass = new UnitTestClass()
            {
                TestDescription = "test3",
                TestId = 3
            };

            var persistenceCacheStackEntity = new PersistenceCacheStackEntity("test3", testClass, null);

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
            var testClass = new UnitTestClass()
            {
                TestDescription = "test4",
                TestId = 4
            };

            var persistenceCacheStackEntity = new PersistenceCacheStackEntity("test4", testClass, null);

            var addResult = GlobalCachingProvider.Instance.AddItem(persistenceCacheStackEntity);
            Assert.IsTrue(addResult);

            var objectCached = GlobalCachingProvider.Instance.GetItem(persistenceCacheStackEntity.Key, false);
            Assert.AreEqual(objectCached.Key, "test4");

            testClass.TestDescription = "update_test4";

            persistenceCacheStackEntity = new PersistenceCacheStackEntity("test4", testClass, null);

            var updateResult = GlobalCachingProvider.Instance.AddItem(persistenceCacheStackEntity);
            Assert.IsTrue(updateResult);

            var updatedObject = GlobalCachingProvider.Instance.GetItem(persistenceCacheStackEntity.Key, false);
            Assert.IsNotNull(updatedObject);
            Assert.IsNotNull(updatedObject.Object);

            var updateTestClass = (UnitTestClass) updatedObject.Object;
            Assert.IsNotNull(updateTestClass);
            Assert.IsTrue(updateTestClass.TestDescription == "update_test4");
        }
    }
}
