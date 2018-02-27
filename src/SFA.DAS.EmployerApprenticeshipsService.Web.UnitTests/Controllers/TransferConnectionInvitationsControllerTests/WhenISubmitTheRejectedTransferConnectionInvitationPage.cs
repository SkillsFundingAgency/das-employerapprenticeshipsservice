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
    public class WhenISubmitTheRejectedTransferConnectionInvitationPage
    {
        private TransferConnectionInvitationsController _controller;
        private readonly RejectedTransferConnectionInvitationViewModel _viewModel = new RejectedTransferConnectionInvitationViewModel();
        private readonly Mock<IMediator> _mediator = new Mock<IMediator>();

        [SetUp]
        public void Arrange()
        {_controller = new TransferConnectionInvitationsController(Mapper.Instance, _mediator.Object);
        }

        [Test]
        public void ThenIShouldBeRedirectedToTheTransfersPageIfIChoseOption1()
        {
            _viewModel.Choice = "GoToTransfersPage";

            var result = _controller.Rejected(_viewModel) as RedirectToRouteResult;

            Assert.That(result, Is.Not.Null);
            Assert.That(result.RouteValues.TryGetValue("action", out var actionName), Is.True);
            Assert.That(actionName, Is.EqualTo("Index"));
            Assert.That(result.RouteValues.TryGetValue("controller", out var controllerName), Is.True);
            Assert.That(controllerName, Is.EqualTo("Transfers"));
        }

        [Test]
        public void ThenIShouldBeRedirectedToTheHomepageIfIChoseOption2()
        {
            _viewModel.Choice = "GoToHomepage";

            var result = _controller.Rejected(_viewModel) as RedirectToRouteResult;

            Assert.That(result, Is.Not.Null);
            Assert.That(result.RouteValues.TryGetValue("action", out var actionName), Is.True);
            Assert.That(actionName, Is.EqualTo("Index"));
            Assert.That(result.RouteValues.TryGetValue("controller", out var controllerName), Is.True);
            Assert.That(controllerName, Is.EqualTo("EmployerTeam"));
        }
    }
}