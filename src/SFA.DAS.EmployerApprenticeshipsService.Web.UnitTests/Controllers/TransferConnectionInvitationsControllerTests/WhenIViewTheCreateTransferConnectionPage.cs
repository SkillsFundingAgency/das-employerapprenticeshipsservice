using System.Web.Mvc;
using AutoMapper;
using MediatR;
using Moq;
using NUnit.Framework;
using SFA.DAS.EAS.Web.Controllers;
using SFA.DAS.EAS.Web.ViewModels;

namespace SFA.DAS.EAS.Web.UnitTests.Controllers.TransferConnectionInvitationsControllerTests
{
    public class WhenIRequestTheStartTransferConnectionInvitationPage
    {
        private const string HashedAccountId = "FOOBAR";

        private TransferConnectionInvitationsController _controller;
        private readonly Mock<IMediator> _mediator = new Mock<IMediator>();
        private readonly Mock<IMapper> _mapper = new Mock<IMapper>();

        [SetUp]
        public void Arrange()
        {
            _controller = new TransferConnectionInvitationsController(_mapper.Object, _mediator.Object);
        }

        [Test]
        public void ThenIShouldBeShownTheStartTransferConnectionPage()
        {
            var result = _controller.Start(HashedAccountId) as ViewResult;
            var model = result?.Model as StartTransferConnectionInvitationViewModel;

            Assert.That(result, Is.Not.Null);
            Assert.That(result.ViewName, Is.EqualTo(""));
            Assert.That(model, Is.Not.Null);
            Assert.That(model.SenderAccountHashedId, Is.EqualTo(HashedAccountId));
        }
    }
}