using Microsoft.AspNetCore.Http;
using SFA.DAS.EmployerAccounts.Web.StartupExtensions;

namespace SFA.DAS.EmployerAccounts.Web.UnitTests.AppStart
{
    [TestFixture]
    public  class RobotsTextMiddlewareTests
    {
        [Test]
        public async Task When_RobotsTextPathIsSet_ReturnPlainText_AndDontInvokeNext()
        {
            // Arrange
            var context = new DefaultHttpContext();
            context.Request.Path = "/robots.txt";

            var nextMock = new Mock<RequestDelegate>();
            var middleware = new RobotsTextMiddleware(nextMock.Object);

            //act
            await middleware.InvokeAsync(context);
            
            // assert
            Assert.AreEqual("text/plain", context.Response.ContentType);
            nextMock.Verify(n => n.Invoke(context), Times.Never);
        }

        [TestCase("")]
        [TestCase("/other-path")]
        public async Task When_NoRobotsTextPathSet_InvokeNext(string _path)
        {
            // Arrange

            var context = new DefaultHttpContext();
            context.Request.Path = _path;

            var nextMock = new Mock<RequestDelegate>();

            var middleware = new RobotsTextMiddleware(nextMock.Object);

            await middleware.InvokeAsync(context);

            // assert
            nextMock.Verify(n => n.Invoke(context), Times.Once);

        }
        
    }
}
