using System.Web.Mvc;
using MediatR;
using Moq;
using NUnit.Framework;
using SFA.DAS.EAS.Web.Controllers;
using SFA.DAS.EAS.Web.ViewModels.TransferConnectionInvitations;

namespace SFA.DAS.EAS.Web.UnitTests.Controllers.TransferConnectionInvitationsControllerTests
{
    [TestFixture]
    public class WhenISubmitTheTransferConnectionInvitationDeletedPage
    {
        private TransferConnectionInvitationsController _controller;
        private DeletedTransferConnectionInvitationViewModel _viewModel;
        private readonly Mock<IMediator> _mediator = new Mock<IMediator>();

        [SetUp]
        public void Arrange()
        {
            _mediator.Setup(m => m.SendAsync(It.IsAny<IAsyncRequest<long>>()));

            _controller = new TransferConnectionInvitationsController(null, _mediator.Object);

            _viewModel = new DeletedTransferConnectionInvitationViewModel();
        }

        [Test]
        public void ThenIShouldBeRedirectedToTransfersDashboardIfIChoseOption1()
        {
            _viewModel.Choice = "GoToTransferDashboard";

            var result = _controller.Deleted(_viewModel) as RedirectToRouteResult;

            Assert.That(result, Is.Not.Null);
            Assert.That(result.RouteValues.TryGetValue("action", out var actionName), Is.True);
            Assert.That(actionName, Is.EqualTo("Index"));
            Assert.That(result.RouteValues.TryGetValue("controller", out var controllerName), Is.True);
            Assert.That(controllerName, Is.EqualTo("Transfers"));
        }

        [Test]
        public void ThenIShouldBeRedirectedToHomePageIfIChoseOption2()
        {
            _viewModel.Choice = "GoToHomepage";

            var result = _controller.Deleted(_viewModel) as RedirectToRouteResult;

            Assert.That(result, Is.Not.Null);
            Assert.That(result.RouteValues.TryGetValue("action", out var actionName), Is.True);
            Assert.That(actionName, Is.EqualTo("Index"));
            Assert.That(result.RouteValues.TryGetValue("controller", out var controllerName), Is.True);
            Assert.That(controllerName, Is.EqualTo("EmployerTeam"));
        }
   }
}