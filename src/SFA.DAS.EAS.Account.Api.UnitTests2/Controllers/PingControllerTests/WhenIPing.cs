using System.Web.Http.Results;
using NUnit.Framework;
using SFA.DAS.EAS.Account.Api.Controllers;

namespace SFA.DAS.EAS.Account.Api.UnitTests.Controllers.PingControllerTests
{
    [TestFixture]
    public class WhenIPing
    {
        private PingController _controller;

        [SetUp]
        public void Arrange()
        {
            _controller = new PingController();
        }

        [Test]
        public void ThenAnOkResultShouldBeReturned()
        {
            var result = _controller.Get();
            
            Assert.That(result, Is.Not.Null);
            Assert.That(result, Is.TypeOf<OkResult>());
        }
    }
}