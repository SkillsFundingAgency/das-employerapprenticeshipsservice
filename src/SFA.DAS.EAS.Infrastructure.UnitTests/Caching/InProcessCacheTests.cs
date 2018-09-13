using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using SFA.DAS.Caches;

namespace SFA.DAS.EAS.Infrastructure.UnitTests.Caching
{
    [TestFixture]
    public class InProcessCacheTests
    {
        [Test]
        public void Constructor_Valid_ShouldNotThrowException()
        {
            var cache = new InProcessCache();
            Assert.Pass("Did not throw exception");
        }

        [Test]
        public void Set_Valid_ShouldNotThrowException()
        {
            var cache = new InProcessCache();
            cache.Set("abc", this);
        }

        [Test]
        public void Get_OnExistingValue_ShouldReturnStoredObject()
        {
            var cache = new InProcessCache();
            cache.Set("abc", this);

            var actualStoredItem = cache.Get<InProcessCacheTests>("abc");

            Assert.AreEqual(this, actualStoredItem);
        }
    }
}
