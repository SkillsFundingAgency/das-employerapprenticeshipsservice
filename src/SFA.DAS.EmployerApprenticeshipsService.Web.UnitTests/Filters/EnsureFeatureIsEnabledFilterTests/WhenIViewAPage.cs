using System.Web.Mvc;
using System.Web.Routing;
using Moq;
using NUnit.Framework;
using SFA.DAS.EAS.Domain.Interfaces;
using SFA.DAS.EAS.Domain.Models.Authorization;
using SFA.DAS.EAS.Web.Authorization;
using SFA.DAS.EAS.Web.Filters;
using SFA.DAS.EAS.Web.Helpers;
using AuthorizationContext = SFA.DAS.EAS.Domain.Models.Authorization.AuthorizationContext;

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
        private Mock<IFeatureToggleService> _featureToggleService;
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

            _featureToggleService = new Mock<IFeatureToggleService>();
            _authorizationService = new Mock<IAuthorizationService>();
            _authorizationContext = new AuthorizationContext();

            _authorizationService.Setup(m => m.GetAuthorizationContext()).Returns(_authorizationContext);
            _featureToggleService.Setup(f => f.IsFeatureEnabled(ControllerName, ActionName, _authorizationContext)).Returns(true);

            _filter = new ValidateFeatureFilter(() => _featureToggleService.Object, () => _authorizationService.Object);
        }

        [Test]
        public void ThenIShouldBeShownThePageIfTheFeatureIsEnabledAndIAmLoggedIn()
        {
            _filter.OnActionExecuting(_filterContext);

            Assert.That(_filterContext.Result, Is.Null);
        }

        [Test]
        public void ThenIShouldNotBeShownThePageIfTheFeatureIsDisabled()
        {
            _featureToggleService.Setup(f => f.IsFeatureEnabled(ControllerName, ActionName, _authorizationContext)).Returns(false);

            _filter.OnActionExecuting(_filterContext);

            var result = _filterContext.Result as ViewResult;

            Assert.That(result, Is.Not.Null);
            Assert.That(result.ViewName, Is.EqualTo(ControllerConstants.FeatureNotEnabledViewName));
        }
    }
}