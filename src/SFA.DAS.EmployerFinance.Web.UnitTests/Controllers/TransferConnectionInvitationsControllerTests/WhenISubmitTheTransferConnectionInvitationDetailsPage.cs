using System.Threading.Tasks;
using MediatR;
using Moq;
using NUnit.Framework;
using SFA.DAS.EmployerFinance.Commands.DeleteSentTransferConnectionInvitation;
using SFA.DAS.EmployerFinance.Web.Controllers;
using SFA.DAS.EmployerFinance.Web.ViewModels;

namespace SFA.DAS.EmployerFinance.Web.UnitTests.Controllers.TransferConnectionInvitationsControllerTests
{
    [TestFixture]
    public class WhenISubmitTheTransferConnectionInvitationDetailsPage
    {
        private const long TransferConnectionId = 123;

        private TransferConnectionInvitationsController _controller;
        private TransferConnectionInvitationViewModel _viewModel;
        private Mock<IMediator> _mediator;

        [SetUp]
        public void Arrange()
        {
            _mediator = new Mock<IMediator>();
            _mediator.Setup(m => m.SendAsync(It.IsAny<IAsyncRequest<long>>()));

            _controller = new TransferConnectionInvitationsController(null, _mediator.Object);

            _viewModel = new TransferConnectionInvitationViewModel
            {
                TransferConnectionInvitationId = 123
            };
        }

        [Test]
        public async Task ThenADeleteTransferConnectionInvitationCommandShouldBeSentIfIChoseOption1()
        {
           _viewModel.Choice = "Confirm";

            await _controller.Details(_viewModel);

            _mediator.Verify(m => m.SendAsync(It.Is<DeleteTransferConnectionInvitationCommand>(c => c.TransferConnectionInvitationId == _viewModel.TransferConnectionInvitationId)), Times.Once);
        }

        [Test]
        public async Task ThenIShouldBeRedirectedToDeleteConfirmedPageIfIChoseOption1()
        {
            _viewModel.Choice = "Confirm";

            var result = await _controller.Details(_viewModel) as RedirectToRouteResult;

            Assert.That(result, Is.Not.Null);
            Assert.That(result.RouteValues.TryGetValue("action", out var actionName), Is.True);
            Assert.That(actionName, Is.EqualTo("Deleted"));
            Assert.That(result.RouteValues.TryGetValue("controller", out var controllerName), Is.False);
        }

        [Test]
        public async Task ThenADeleteTransferConnectionInvitationCommandShouldNotBeSentIfIChoseOption2()
        {
            _viewModel.Choice = "GoToTransfersPage";

            await _controller.Details(_viewModel);

            _mediator.Verify(m => m.SendAsync(It.IsAny<DeleteTransferConnectionInvitationCommand>()), Times.Never);
        }

        [Test]
        public async Task ThenIShouldBeRedirectedToTheTransfersPageIfIChoseOption2()
        {
            _viewModel.Choice = "GoToTransfersPage";

            var result = await _controller.Details(_viewModel) as RedirectToRouteResult;

            Assert.That(result, Is.Not.Null);
            Assert.That(result.RouteValues.TryGetValue("action", out var actionName), Is.True);
            Assert.That(actionName, Is.EqualTo("Index"));
            Assert.That(result.RouteValues.TryGetValue("controller", out var controllerName), Is.True);
            Assert.That(controllerName, Is.EqualTo("TransferConnections"));
        }
    }
}