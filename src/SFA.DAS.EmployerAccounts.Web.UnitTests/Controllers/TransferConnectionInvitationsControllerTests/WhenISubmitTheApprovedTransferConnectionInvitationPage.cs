using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using MediatR;
using Moq;
using NUnit.Framework;
using SFA.DAS.EmployerAccounts.Web.Controllers;
using SFA.DAS.EmployerAccounts.Web.Helpers;
using SFA.DAS.EmployerAccounts.Web.ViewModels;

namespace SFA.DAS.EmployerAccounts.Web.UnitTests.Controllers.TransferConnectionInvitationsControllerTests
{
    [TestFixture]
    public class WhenISubmitTheApprovedTransferConnectionInvitationPage
    {
        private const string AccountHashedId = "ABC123";

        private TransferConnectionInvitationsController _controller;
        private readonly ApprovedTransferConnectionInvitationViewModel _viewModel = new ApprovedTransferConnectionInvitationViewModel();
        private readonly Mock<IMediator> _mediator = new Mock<IMediator>();

        [SetUp]
        public void Arrange()
        {
            var routeData = new RouteData();

            routeData.Values[ControllerConstants.AccountHashedIdRouteKeyName] = AccountHashedId;

            var urlHelper = new UrlHelper(new RequestContext(Mock.Of<HttpContextBase>(), routeData));

            _controller = new TransferConnectionInvitationsController(null, _mediator.Object) { Url = urlHelper };
        }

        [Test]
        public void ThenIShouldBeRedirectedToTheApprenticesPageIfIChoseOption1()
        {
            _viewModel.Choice = "GoToApprenticesPage";

            var result = _controller.Approved(_viewModel) as RedirectResult;

            Assert.That(result, Is.Not.Null);
            Assert.That(result.Url, Is.EqualTo($"/{AccountHashedId}/apprentices"));
        }

        [Test]
        public void ThenIShouldBeRedirectedToTheHomepageIfIChoseOption2()
        {
            _viewModel.Choice = "GoToHomepage";

            var result = _controller.Approved(_viewModel) as RedirectToRouteResult;

            Assert.That(result, Is.Not.Null);
            Assert.That(result.RouteValues.TryGetValue("action", out var actionName), Is.True);
            Assert.That(actionName, Is.EqualTo("Index"));
            Assert.That(result.RouteValues.TryGetValue("controller", out var controllerName), Is.True);
            Assert.That(controllerName, Is.EqualTo("EmployerTeam"));
        }
    }
}