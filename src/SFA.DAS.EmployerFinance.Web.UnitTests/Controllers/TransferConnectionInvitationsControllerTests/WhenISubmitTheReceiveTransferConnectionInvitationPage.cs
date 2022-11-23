using System.Threading.Tasks;
using System.Web.Mvc;
using MediatR;
using Moq;
using NUnit.Framework;
using SFA.DAS.EmployerFinance.Commands.ApproveTransferConnectionInvitation;
using SFA.DAS.EmployerFinance.Commands.RejectTransferConnectionInvitation;
using SFA.DAS.EmployerFinance.Web.Controllers;
using SFA.DAS.EmployerFinance.Web.ViewModels;

namespace SFA.DAS.EmployerFinance.Web.UnitTests.Controllers.TransferConnectionInvitationsControllerTests
{
    [TestFixture]
    public class WhenISubmitTheReceiveTransferConnectionInvitationPage
    {
        private const int TransferConnectionId = 123;

        private TransferConnectionInvitationsController _controller;
        private ReceiveTransferConnectionInvitationViewModel _viewModel;
        private Mock<IMediator> _mediator;

        [SetUp]
        public void Arrange()
        {
            _mediator = new Mock<IMediator>();

            _controller = new TransferConnectionInvitationsController(null, _mediator.Object);

            _viewModel = new ReceiveTransferConnectionInvitationViewModel
            {
                TransferConnectionInvitationId = TransferConnectionId
            };
        }

        [Test]
        public async Task ThenAnApproveTransferConnectionCommandShouldBeSentIfIChoseOption1()
        {
            _viewModel.Choice = "Approve";

            await _controller.Receive(_viewModel);

            _mediator.Verify(m => m.SendAsync(It.Is<ApproveTransferConnectionInvitationCommand>(c => c.TransferConnectionInvitationId == _viewModel.TransferConnectionInvitationId)), Times.Once);
        }

        [Test]
        public async Task ThenIShouldBeRedirectedToTheApprovedTransferConnectionPageIfIChoseOption1()
        {
            _viewModel.Choice = "Approve";

            var result = await _controller.Receive(_viewModel) as RedirectToRouteResult;

            Assert.That(result, Is.Not.Null);
            Assert.That(result.RouteValues.TryGetValue("action", out var actionName), Is.True);
            Assert.That(actionName, Is.EqualTo("Approved"));
            Assert.That(result.RouteValues.ContainsKey("controller"), Is.False);
            Assert.That(result.RouteValues.TryGetValue("transferConnectionInvitationId", out var transferConnectionId), Is.True);
            Assert.That(transferConnectionId, Is.EqualTo(TransferConnectionId));
        }

        [Test]
        public async Task ThenAnApproveTransferConnectionCommandShouldNotBeSentIfIChoseOption2()
        {
            _viewModel.Choice = "Reject";

            await _controller.Receive(_viewModel);

            _mediator.Verify(m => m.SendAsync(It.IsAny<ApproveTransferConnectionInvitationCommand>()), Times.Never);
        }

        [Test]
        public async Task ThenARejectTransferConnectionCommandShouldBeSentIfIChoseOption2()
        {
            _viewModel.Choice = "Reject";

            await _controller.Receive(_viewModel);

            _mediator.Verify(m => m.SendAsync(It.Is<RejectTransferConnectionInvitationCommand>(c => c.TransferConnectionInvitationId == _viewModel.TransferConnectionInvitationId)), Times.Once);
        }

        [Test]
        public async Task ThenIShouldBeRedirectedToTheRejectedTransferConnectionPageIfIChoseOption2()
        {
            _viewModel.Choice = "Reject";

            var result = await _controller.Receive(_viewModel) as RedirectToRouteResult;

            Assert.That(result, Is.Not.Null);
            Assert.That(result.RouteValues.TryGetValue("action", out var actionName), Is.True);
            Assert.That(actionName, Is.EqualTo("Rejected"));
            Assert.That(result.RouteValues.ContainsKey("controller"), Is.False);
            Assert.That(result.RouteValues.TryGetValue("transferConnectionInvitationId", out var transferConnectionId), Is.True);
            Assert.That(transferConnectionId, Is.EqualTo(TransferConnectionId));
        }

        [Test]
        public async Task ThenARejectTransferConnectionCommandShouldNotBeSentIfIChoseOption1()
        {
            _viewModel.Choice = "Approve";

            await _controller.Receive(_viewModel);

            _mediator.Verify(m => m.SendAsync(It.IsAny<RejectTransferConnectionInvitationCommand>()), Times.Never);
        }
    }
}