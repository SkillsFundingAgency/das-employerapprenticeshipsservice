using NUnit.Framework;
using SFA.DAS.EAS.Web.DependencyResolution;
using SFA.DAS.StuctureMap.Extensions;
using StructureMap;
using System.IO;
using System.Web;

namespace SFA.DAS.EAS.Web.UnitTests.DependencyResolution.DefaultRegistryTests
{
    class WhenTheHttpContextBaseIsResolved
    {
        [Test]
        public void AndTheCurrentHttpContextHasBeenDisposed_ThenTheNullHttpContextIsReturned()
        {
            // arrange
            var sut = new Container(c => c.AddRegistry<DefaultRegistry>());
            HttpContext.Current = null;

            // act
            var context = sut.GetInstance<HttpContextBase>();

            // assert
            Assert.IsAssignableFrom<NullHttpContext>(context);
        }

        [Test]
        public void AndTheHttpContextExists_ThenTheHttpContextWrapperIsReturned()
        {
            // arrange
            var sut = new Container(c => c.AddRegistry<DefaultRegistry>());
            HttpContext.Current = new HttpContext(new HttpRequest("", "http://localhost/", ""), new HttpResponse(new StringWriter())); ;

            // act
            var context = sut.GetInstance<HttpContextBase>();

            // assert
            Assert.IsAssignableFrom<HttpContextWrapper>(context);
        }
    }
}
