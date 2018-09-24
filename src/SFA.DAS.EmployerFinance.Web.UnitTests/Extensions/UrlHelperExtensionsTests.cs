using NUnit.Framework;
using NUnit.Framework.Internal;
using SFA.DAS.EmployerFinance.Web.Extensions;
using System.Web.Mvc;

namespace SFA.DAS.EmployerFinance.Web.UnitTests.Extensions
{
    [TestFixture()]
    public class UrlHelperExtensionsTests
    {

        [Test]
        public void RemoveUrlHttpScheme()
        {
            const string urlScheme = "http://";
            const string urlPath = "www.test.com";
            const string url = urlScheme + urlPath;
            var helper = new UrlHelper();

            var result = helper.RemoveHttpScheme(url);

            Assert.AreEqual(urlPath, result);
        }

        [Test]
        public void RemoveUrlHttpsScheme()
        {
            const string urlScheme = "https://";
            const string urlPath = "www.test.com";
            const string url = urlScheme + urlPath;
            var helper = new UrlHelper();

            var result = helper.RemoveHttpScheme(url);

            Assert.AreEqual(urlPath, result);
        }

        [Test]
        public void RemoveUrlNoScheme()
        {
            const string urlPath = "www.test.com";

            var helper = new UrlHelper();

            var result = helper.RemoveHttpScheme(urlPath);

            Assert.AreEqual(urlPath, result);
        }
    }
}
