using System.Web.Mvc;
using AutoMapper;
using MediatR;
using Moq;
using NUnit.Framework;
using SFA.DAS.EAS.Web.Controllers;
using SFA.DAS.EAS.Web.ViewModels;

namespace SFA.DAS.EAS.Web.UnitTests.Controllers.TransferConnectionControllerTests
{
    public class WhenIRequestTheCreateTransferConnectionPage
    {
        private const string HashedAccountId = "FOOBAR";

        private TransferConnectionInvitationController _controller;
        private readonly Mock<IMediator> _mediator = new Mock<IMediator>();
        private readonly Mock<IMapper> _mapper = new Mock<IMapper>();

        [SetUp]
        public void Arrange()
        {
            _controller = new TransferConnectionInvitationController(_mapper.Object, _mediator.Object);
        }

        [Test]
        public void ThenIShouldBeShownTheCreateTransferConnectionPage()
        {
            var result = _controller.Create(HashedAccountId) as ViewResult;
            var model = result?.Model as CreateTransferConnectionInvitationViewModel;

            Assert.That(result, Is.Not.Null);
            Assert.That(result.ViewName, Is.EqualTo(""));
            Assert.That(model, Is.Not.Null);
            Assert.That(model.SenderHashedAccountId, Is.EqualTo(HashedAccountId));
        }
    }
}