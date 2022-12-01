using System.Threading.Tasks;
using System.Web.Mvc;
using MediatR;
using Moq;
using NUnit.Framework;
using SFA.DAS.EmployerFinance.Commands.SendTransferConnectionInvitation;
using SFA.DAS.EmployerFinance.Web.Controllers;
using SFA.DAS.EmployerFinance.Web.ViewModels;

namespace SFA.DAS.EmployerFinance.Web.UnitTests.Controllers.TransferConnectionInvitationsControllerTests
{
    [TestFixture]
    public class WhenISubmitTheSendTransferConnectionInvitationPage
    {
        private const long TransferConnectionId = 123;

        private TransferConnectionInvitationsController _controller;
        private SendTransferConnectionInvitationViewModel _viewModel;
        private Mock<IMediator> _mediator;

        [SetUp]
        public void Arrange()
        {
            _mediator = new Mock<IMediator>();
            _mediator.Setup(m => m.SendAsync(It.IsAny<IAsyncRequest<long>>())).ReturnsAsync(TransferConnectionId);

            _controller = new TransferConnectionInvitationsController(null, _mediator.Object);

            _viewModel = new SendTransferConnectionInvitationViewModel
            {
                ReceiverAccountPublicHashedId = "ABC123"
            };
        }

        [Test]
        public async Task ThenASendTransferConnectionCommandShouldBeSentIfIChoseOption1()
        {
            _viewModel.Choice = "Confirm";

            await _controller.Send(_viewModel);

            _mediator.Verify(m => m.SendAsync(It.Is<SendTransferConnectionInvitationCommand>(c => c.ReceiverAccountPublicHashedId == _viewModel.ReceiverAccountPublicHashedId)), Times.Once);
        }

        [Test]
        public async Task ThenIShouldBeRedirectedToTheSentTransferConnectionPageIfIChoseOption1()
        {
            _viewModel.Choice = "Confirm";

            var result = await _controller.Send(_viewModel) as RedirectToRouteResult;

            Assert.That(result, Is.Not.Null);
            Assert.That(result.RouteValues.TryGetValue("action", out var actionName), Is.True);
            Assert.That(actionName, Is.EqualTo("Sent"));
            Assert.That(result.RouteValues.ContainsKey("controller"), Is.False);
            Assert.That(result.RouteValues.TryGetValue("transferConnectionInvitationId", out var transferConnectionId), Is.True);
            Assert.That(transferConnectionId, Is.EqualTo(TransferConnectionId));
        }

        [Test]
        public async Task ThenASendTransferConnectionCommandShouldNotBeSentIfIChoseOption2()
        {
            _viewModel.Choice = "ReEnterAccountId";

            await _controller.Send(_viewModel);

            _mediator.Verify(m => m.SendAsync(It.IsAny<SendTransferConnectionInvitationCommand>()), Times.Never);
        }

        [Test]
        public async Task ThenIShouldBeRedirectedToStartPageIfIChoseOption2()
        {
            _viewModel.Choice = "ReEnterAccountId";

            var result = await _controller.Send(_viewModel) as RedirectToRouteResult;

            Assert.That(result, Is.Not.Null);
            Assert.That(result.RouteValues.TryGetValue("action", out var actionName), Is.True);
            Assert.That(actionName, Is.EqualTo("Start"));
            Assert.That(result.RouteValues.ContainsKey("controller"), Is.False);
        }
    }
}