using NUnit.Framework;
using System.IO;
using System.Web;

namespace SFA.DAS.StuctureMap.Extensions.UnitTests.ToContextBaseTests
{
    class WhenToHttpContextBaseIsCalled
    {
        [Test]
        public void AndTheHttpContextIsNull_ThenTheNullHttpContextIsReturned()
        {
            // arrange
            HttpContext.Current = null;

            // act
            var context = HttpContextExtensions.ToHttpContextBase(HttpContext.Current);

            // assert
            Assert.IsAssignableFrom<NullHttpContext>(context);
        }

        [Test]
        public void AndTheCurrentHttpContextIsNull_ThenTheNullHttpContextContainsOneItem()
        {
            // arrange
            HttpContext.Current = null;

            // act
            var context = HttpContextExtensions.ToHttpContextBase(HttpContext.Current);

            // assert
            Assert.AreEqual(1, context.Items.Count);
        }

        [Test]
        public void AndTheCurrentHttpContextIsNull_ThenTheNullHttpContextContainsANullNestedContainer()
        {
            // arrange
            HttpContext.Current = null;

            // act
            var context = HttpContextExtensions.ToHttpContextBase(HttpContext.Current);

            // assert
            Assert.IsNull(context.Items["Nested.Container.Key"]);
        }

        [Test]
        public void AndTheHttpContextIsNotNull_ThenTheHttpContextWrapperIsReturned()
        {
            // arrange
            HttpContext.Current = new HttpContext(new HttpRequest("", "http://localhost/", ""), new HttpResponse(new StringWriter())); ;

            // act
            var context = HttpContextExtensions.ToHttpContextBase(HttpContext.Current);

            // assert
            Assert.IsAssignableFrom<HttpContextWrapper>(context);
        }
    }
}
