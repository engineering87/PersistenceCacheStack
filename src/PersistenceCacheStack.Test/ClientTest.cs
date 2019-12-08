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

using Microsoft.VisualStudio.TestTools.UnitTesting;
using PersistenceCacheStack.Entities;
using PersistenceCacheStack.RedisCache;
using PersistenceCacheStack.Test.TestObject;

namespace PersistenceCacheStack.Test
{
    [TestClass]
    public class ClientTest
    {
        [TestMethod]
        public void TestAddItem()
        {
            var testClass = new UnitTestClass()
            {
                TestDescription = "TestAddItem",
                TestId = 1
            };

            var persistenceCacheStack = new PersistenceCacheStackClient<UnitTestClass>(false);

            var addResult = persistenceCacheStack.AddItem("TestAddItem", testClass);
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

            var persistenceCacheStack = new PersistenceCacheStackClient<UnitTestClass>(false);

            var addResult = persistenceCacheStack.AddItem("TestAddItem", testClass);
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

            var persistenceCacheStack = new PersistenceCacheStackClient<UnitTestClass>(false);

            var addResult = persistenceCacheStack.AddItem("TestRemoveItem", testClass);
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

            var persistenceCacheStackEntity = new StackEntity("TestSynchFromRedis", testClass);

            var redisWrapper = new RedisWrapper();

            // add fake object into Redis
            var addResult = redisWrapper.Insert(persistenceCacheStackEntity);
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
