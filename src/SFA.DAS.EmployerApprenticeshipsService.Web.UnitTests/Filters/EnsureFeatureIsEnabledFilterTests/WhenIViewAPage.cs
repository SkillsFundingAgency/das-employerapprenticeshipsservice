using System.Web.Mvc;
using System.Web.Routing;
using Moq;
using NUnit.Framework;
using SFA.DAS.EAS.Domain.Interfaces;
using SFA.DAS.EAS.Domain.Models.Authorization;
using SFA.DAS.EAS.Web.Filters;
using SFA.DAS.EAS.Web.Helpers;


namespace SFA.DAS.EAS.Web.UnitTests.Filters.EnsureFeatureIsEnabledFilterTests
{
    [TestFixture]
    public class WhenIViewAPage
    {
        private const string ControllerName = "Foo";
        private const string ActionName = "Bar";

        private ValidateFeatureFilter _filter;
        private ActionExecutingContext _filterContext;
        private RouteData _routeData;
        private readonly ControllerBase _controller = Mock.Of<ControllerBase>();
        private Mock<IAuthorizationService> _authorizationService;
        private IAuthorizationContext _authorizationContext;
        
        [SetUp]
        public void Arrange()
        {
            _routeData = new RouteData();

            _routeData.Values.Add(ControllerConstants.ControllerKeyName, ControllerName);
            _routeData.Values.Add(ControllerConstants.ActionKeyName, ActionName);

            _filterContext = new ActionExecutingContext
            {
                RouteData = _routeData,
                Controller = _controller
            };

            _authorizationService = new Mock<IAuthorizationService>();
            _authorizationContext = new Domain.Models.Authorization.AuthorizationContext();

            _authorizationService.Setup(m => m.GetAuthorizationContext()).Returns(_authorizationContext);

            _filter = new ValidateFeatureFilter(() => _authorizationService.Object);
        }

        [Test]
        public void ThenIShouldBeShownThePageIfTheFeatureIsEnabledAndIAmLoggedIn()
        {
	        _authorizationService.Setup(authservice => authservice.IsOperationAuthorised()).Returns(true);
            _filter.OnActionExecuting(_filterContext);

            Assert.That(_filterContext.Result, Is.Null);
        }

		[Test]
        public void ThenIShouldNotBeShownThePageIfTheFeatureIsDisabled()
        {
            _authorizationService.Setup(f => f.IsOperationAuthorised()).Returns(false);

            _filter.OnActionExecuting(_filterContext);

            var result = _filterContext.Result as ViewResult;

            Assert.That(result, Is.Not.Null);
            Assert.That(result.ViewName, Is.EqualTo(ControllerConstants.FeatureNotEnabledViewName));
        }
    }
}