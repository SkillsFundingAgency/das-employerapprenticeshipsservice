using NUnit.Framework;
using SFA.DAS.EmployerFinance.Web.Controllers;
using System.Web.Mvc;

namespace SFA.DAS.EmployerFinance.Web.UnitTests.Controllers.TransfersControllerTests
{
    [TestFixture]
    public class WhenIViewTheTransfersPage
    {
        private TransfersController _controller;

        [SetUp]
        public void Arrange()
        {
            _controller = new TransfersController(null, null, null);
        }

        [Test]
        public void ThenIShouldBeShownTheTransfersPage()
        {
            var result = _controller.Index() as ViewResult;

            Assert.That(result, Is.Not.Null);
            Assert.That(result.ViewName, Is.EqualTo(""));
            Assert.That(result.Model, Is.Null);
        }
    }
}