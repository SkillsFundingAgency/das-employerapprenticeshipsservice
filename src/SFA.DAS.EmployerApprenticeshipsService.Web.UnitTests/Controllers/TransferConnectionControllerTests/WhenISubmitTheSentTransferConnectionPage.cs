using System.Web.Mvc;
using MediatR;
using Moq;
using NUnit.Framework;
using SFA.DAS.EAS.Application.Queries.GetSentTransferConnectionQuery;
using SFA.DAS.EAS.Web.Controllers;

namespace SFA.DAS.EAS.Web.UnitTests.Controllers.TransferConnectionControllerTests
{
    public class WhenISubmitTheSentTransferConnectionPage
    {
        private TransferConnectionController _controller;
        private readonly SentTransferConnectionViewModel _viewModel = new SentTransferConnectionViewModel();
        private readonly Mock<IMediator> _mediator = new Mock<IMediator>();

        [SetUp]
        public void Arrange()
        {
            _controller = new TransferConnectionController(_mediator.Object);
        }

        [Test]
        public void ThenIShouldBeRedirectedToTheTransfersPageIfIChoseOption1()
        {
            _viewModel.Choice = 1;

            var result = _controller.Sent(_viewModel) as RedirectToRouteResult;

            Assert.That(result, Is.Not.Null);
            Assert.That(result.RouteValues.TryGetValue("action", out var actionName), Is.True);
            Assert.That(actionName, Is.EqualTo("Index"));
            Assert.That(result.RouteValues.TryGetValue("controller", out var controllerName), Is.True);
            Assert.That(controllerName, Is.EqualTo("Transfer"));
        }

        [Test]
        public void ThenIShouldBeRedirectedToTheHomepageIfIChoseOption2()
        {
            _viewModel.Choice = 2;

            var result = _controller.Sent(_viewModel) as RedirectToRouteResult;

            Assert.That(result, Is.Not.Null);
            Assert.That(result.RouteValues.TryGetValue("action", out var actionName), Is.True);
            Assert.That(actionName, Is.EqualTo("Index"));
            Assert.That(result.RouteValues.TryGetValue("controller", out var controllerName), Is.True);
            Assert.That(controllerName, Is.EqualTo("EmployerTeam"));
        }
    }
}