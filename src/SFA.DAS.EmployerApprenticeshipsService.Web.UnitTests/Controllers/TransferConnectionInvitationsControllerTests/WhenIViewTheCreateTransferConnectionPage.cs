using System.Web.Mvc;
using AutoMapper;
using MediatR;
using Moq;
using NUnit.Framework;
using SFA.DAS.EAS.Web.Controllers;
using SFA.DAS.EAS.Web.ViewModels.TransferConnectionInvitations;

namespace SFA.DAS.EAS.Web.UnitTests.Controllers.TransferConnectionInvitationsControllerTests
{
    [TestFixture]
    public class WhenIRequestTheStartTransferConnectionInvitationPage
    {
        private TransferConnectionInvitationsController _controller;
        private readonly Mock<IMediator> _mediator = new Mock<IMediator>();

        [SetUp]
        public void Arrange()
        {
            _controller = new TransferConnectionInvitationsController(Mapper.Instance, _mediator.Object);
        }

        [Test]
        public void ThenIShouldBeShownTheStartTransferConnectionPage()
        {
            var result = _controller.Start() as ViewResult;
            var model = result?.Model as StartTransferConnectionInvitationViewModel;

            Assert.That(result, Is.Not.Null);
            Assert.That(result.ViewName, Is.EqualTo(""));
            Assert.That(model, Is.Not.Null);
        }
    }
}