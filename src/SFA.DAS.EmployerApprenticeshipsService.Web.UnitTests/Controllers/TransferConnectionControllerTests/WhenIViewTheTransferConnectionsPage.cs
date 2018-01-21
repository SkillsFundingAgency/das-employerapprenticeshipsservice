using System.Web.Mvc;
using MediatR;
using Moq;
using NUnit.Framework;
using SFA.DAS.EAS.Web.Controllers;

namespace SFA.DAS.EAS.Web.UnitTests.Controllers.TransferConnectionControllerTests
{
    public class WhenIViewTheTransferConnectionsPage
    {
        private TransferConnectionController _controller;
        private readonly Mock<IMediator> _mediator = new Mock<IMediator>();

        [SetUp]
        public void Arrange()
        {
            _controller = new TransferConnectionController(_mediator.Object);
        }

        [Test]
        public void ThenIShouldBeShownTheTransferConnectionsPage()
        {
            var result = _controller.Index() as ViewResult;
            var model = result?.Model;

            Assert.That(result, Is.Not.Null);
            Assert.That(result.ViewName, Is.EqualTo(""));
            Assert.That(model, Is.Null);
        }
    }
}