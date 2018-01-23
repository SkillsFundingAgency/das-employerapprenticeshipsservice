using System.Threading.Tasks;
using System.Web.Mvc;
using AutoMapper;
using MediatR;
using Moq;
using NUnit.Framework;
using SFA.DAS.EAS.Web.Controllers;
using SFA.DAS.EAS.Web.ViewModels;

namespace SFA.DAS.EAS.Web.UnitTests.Controllers.TransferConnectionControllerTests
{
    public class WhenISubmitTheCreateTransferConnectionPage
    {
        private const string HashedAccountId = "FOOBAR";
        private const long TransferConnectionId = 123;

        private TransferConnectionInvitationController _controller;
        private readonly CreateTransferConnectionInvitationViewModel _viewModel = new CreateTransferConnectionInvitationViewModel { SenderHashedAccountId = HashedAccountId };
        private readonly Mock<IMediator> _mediator = new Mock<IMediator>();
        private readonly Mock<IMapper> _mapper = new Mock<IMapper>();

        [SetUp]
        public void Arrange()
        {
            _mediator.Setup(m => m.SendAsync(It.IsAny<IAsyncRequest<long>>())).ReturnsAsync(TransferConnectionId);

            _controller = new TransferConnectionInvitationController(_mapper.Object, _mediator.Object);
        }

        [Test]
        public async Task ThenACreateTransferConnectionCommandShouldBeSent()
        {
            await _controller.Create(_viewModel);

            _mediator.Verify(m => m.SendAsync(_viewModel.Message), Times.Once);
        }

        [Test]
        public async Task ThenIShouldBeRedirectedToTheSendTransferConnectionPage()
        {
            var result = await _controller.Create(_viewModel) as RedirectToRouteResult;

            Assert.That(result, Is.Not.Null);
            Assert.That(result.RouteValues.TryGetValue("action", out var actionName), Is.True);
            Assert.That(actionName, Is.EqualTo("Send"));
            Assert.That(result.RouteValues.ContainsKey("controller"), Is.False);
            Assert.That(result.RouteValues.TryGetValue("TransferConnectionInvitationId", out var transferConnectionId), Is.True);
            Assert.That(transferConnectionId, Is.EqualTo(TransferConnectionId));
        }
    }
}