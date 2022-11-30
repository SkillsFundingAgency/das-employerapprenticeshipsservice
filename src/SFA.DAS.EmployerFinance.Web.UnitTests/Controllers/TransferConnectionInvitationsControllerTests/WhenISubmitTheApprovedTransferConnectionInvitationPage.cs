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
            Assert.That(result.Url, Is.EqualTo($"/{AccountHashedId}"));
        }

        [Test]
        public void ThenIShouldBeRedirectedToTheHomepageIfIChoseOption2()
        {
            _viewModel.Choice = "GoToHomepage";

            var result = _controller.Approved(_viewModel) as RedirectResult;

            Assert.That(result, Is.Not.Null);
            Assert.That(result.Url, Is.EqualTo($"/accounts/{AccountHashedId}/teams"));
        }
    }
}