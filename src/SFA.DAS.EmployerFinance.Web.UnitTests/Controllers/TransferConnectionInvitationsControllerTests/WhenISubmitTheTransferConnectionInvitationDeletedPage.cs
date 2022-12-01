using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using MediatR;
using Moq;
using NUnit.Framework;
using SFA.DAS.EmployerFinance.Web.Controllers;
using SFA.DAS.EmployerFinance.Web.Helpers;
using SFA.DAS.EmployerFinance.Web.ViewModels;

namespace SFA.DAS.EmployerFinance.Web.UnitTests.Controllers.TransferConnectionInvitationsControllerTests
{
    [TestFixture]
    public class WhenISubmitTheTransferConnectionInvitationDeletedPage
    {
        private const string AccountHashedId = "ABC123";

        private TransferConnectionInvitationsController _controller;
        private DeletedTransferConnectionInvitationViewModel _viewModel = new DeletedTransferConnectionInvitationViewModel();
        private readonly Mock<IMediator> _mediator = new Mock<IMediator>();

        [SetUp]
        public void Arrange()
        {
            var routeData = new RouteData();

            routeData.Values[ControllerConstants.AccountHashedIdRouteKeyName] = AccountHashedId;

            var urlHelper = new UrlHelper(new RequestContext(Mock.Of<HttpContextBase>(), routeData));

            _mediator.Setup(m => m.SendAsync(It.IsAny<IAsyncRequest<long>>()));

            _controller = new TransferConnectionInvitationsController(null, _mediator.Object) { Url = urlHelper };
        }

        [Test]
        public void ThenIShouldBeRedirectedToTransfersDashboardIfIChoseOption1()
        {
            _viewModel.Choice = "GoToTransfersPage";

            var result = _controller.Deleted(_viewModel) as RedirectToRouteResult;

            Assert.That(result, Is.Not.Null);
            Assert.That(result.RouteValues.TryGetValue("action", out var actionName), Is.True);
            Assert.That(actionName, Is.EqualTo("Index"));
            Assert.That(result.RouteValues.TryGetValue("controller", out var controllerName), Is.True);
            Assert.That(controllerName, Is.EqualTo("TransferConnections"));
        }

        [Test]
        public void ThenIShouldBeRedirectedToHomePageIfIChoseOption2()
        {
            _viewModel.Choice = "GoToHomepage";

            var result = _controller.Deleted(_viewModel) as RedirectResult;

            Assert.That(result, Is.Not.Null);
            Assert.That(result.Url, Is.EqualTo($"/accounts/{AccountHashedId}/teams"));
        }
    }
}