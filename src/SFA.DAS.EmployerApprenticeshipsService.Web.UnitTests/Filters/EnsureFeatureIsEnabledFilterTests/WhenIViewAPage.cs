using System.Web.Mvc;
using System.Web.Routing;
using Moq;
using NUnit.Framework;
using SFA.DAS.EAS.Domain.Interfaces;
using SFA.DAS.EAS.Web.Authentication;
using SFA.DAS.EAS.Web.Filters;
using SFA.DAS.EAS.Web.Helpers;

namespace SFA.DAS.EAS.Web.UnitTests.Filters.EnsureFeatureIsEnabledFilterTests
{
    [TestFixture]
    public class WhenIViewAPage
    {
        private const string ControllerName = "Foo";
        private const string ActionName = "Bar";
        private const string CurrentUserEmail = "foo@bar.test";

        private EnsureFeatureIsEnabledFilter _filter;
        private ActionExecutingContext _filterContext;
        private RouteData _routeData;
        private Mock<ICurrentUserService> _currentUserService;
        private Mock<IFeatureToggleService> _featureToggleService;
        private CurrentUser _currentUser;
        private readonly ControllerBase _controller = Mock.Of<ControllerBase>();
        
        [SetUp]
        public void Arrange()
        {
            _currentUserService = new Mock<ICurrentUserService>();
            _featureToggleService = new Mock<IFeatureToggleService>();
            _routeData = new RouteData();

            _routeData.Values.Add(ControllerConstants.ControllerKeyName, ControllerName);
            _routeData.Values.Add(ControllerConstants.ActionKeyName, ActionName);

            _filterContext = new ActionExecutingContext
            {
                RouteData = _routeData,
                Controller = _controller
            };
            
            _filter = new EnsureFeatureIsEnabledFilter(() => _currentUserService.Object, () => _featureToggleService.Object);
        }

        [Test]
        public void ThenIShouldBeShownThePageIfTheFeatureIsEnabledAndIAmLoggedIn()
        {
            _currentUser = new CurrentUser { Email = CurrentUserEmail };
            _featureToggleService.Setup(f => f.IsFeatureEnabled(ControllerName, ActionName, CurrentUserEmail)).Returns(true);
            _currentUserService.Setup(c => c.GetCurrentUser()).Returns(_currentUser);

            _filter.OnActionExecuting(_filterContext);

            Assert.That(_filterContext.Result, Is.Null);
        }

        [Test]
        public void ThenIShouldNotBeShownThePageIfTheFeatureIsNotEnabledAndIAmLoggedIn()
        {
            _featureToggleService.Setup(f => f.IsFeatureEnabled(ControllerName, ActionName, CurrentUserEmail)).Returns(false);
            _currentUserService.Setup(c => c.GetCurrentUser()).Returns(_currentUser);

            _filter.OnActionExecuting(_filterContext);

            var result = _filterContext.Result as ViewResult;

            Assert.That(result, Is.Not.Null);
            Assert.That(result.ViewName, Is.EqualTo(ControllerConstants.FeatureNotEnabledViewName));
        }

        [Test]
        public void ThenIShouldBeShownThePageIfTheFeatureIsEnabledAndIAmNotLoggedIn()
        {
            _currentUser = null;

            _featureToggleService.Setup(f => f.IsFeatureEnabled(ControllerName, ActionName, null)).Returns(true);
            _currentUserService.Setup(c => c.GetCurrentUser()).Returns(_currentUser);

            _filter.OnActionExecuting(_filterContext);

            Assert.That(_filterContext.Result, Is.Null);
        }

        [Test]
        public void ThenIShouldNotBeShownThePageIfTheFeatureIsNotEnabledAndIAmNotLoggedIn()
        {
            _currentUser = null;
            
            _featureToggleService.Setup(f => f.IsFeatureEnabled(ControllerName, ActionName, null)).Returns(false);
            _currentUserService.Setup(c => c.GetCurrentUser()).Returns(_currentUser);

            _filter.OnActionExecuting(_filterContext);

            var result = _filterContext.Result as ViewResult;

            Assert.That(result, Is.Not.Null);
            Assert.That(result.ViewName, Is.EqualTo(ControllerConstants.FeatureNotEnabledViewName));
        }
    }
}