using System;
using Moq;
using NUnit.Framework;
using SFA.DAS.EAS.Domain.Interfaces;
using SFA.DAS.EAS.Domain.Models.UserProfile;
using SFA.DAS.EAS.Web.Authentication;
using SFA.DAS.EAS.Web.Filters;
using SFA.DAS.EAS.Web.Helpers;
using System.Web.Mvc;
using System.Web.Routing;

namespace SFA.DAS.EAS.Web.UnitTests.Filters
{
    [TestFixture]
    public class WhenIViewAPage
    {
        private const string ControllerName = "Foo";
        private const string ActionName = "Bar";

        private EnsureFeatureIsEnabledFilter _filter;
        private ActionExecutingContext _filterContext;
        private RouteData _routeData;
        private readonly ControllerBase _controller = Mock.Of<ControllerBase>();
        private Mock<ICurrentUserService> _currentUserService;
        private Mock<IFeatureToggleService> _featureToggleService;
        private CurrentUser _currentUser;

        [SetUp]
        public void Arrange()
        {
            _currentUser = new CurrentUser
            {
                ExternalUserId = Guid.NewGuid().ToString(),
                Email = "foo@bar.test"
            };

            _currentUserService = new Mock<ICurrentUserService>();
            _featureToggleService = new Mock<IFeatureToggleService>();
            
            _currentUserService.Setup(c => c.GetCurrentUser()).Returns(_currentUser);

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
            _currentUserService.Setup(c => c.GetCurrentUser()).Returns(_currentUser);
            _featureToggleService.Setup(f => f.IsFeatureEnabled(ControllerName, ActionName, _currentUser.ExternalUserId, _currentUser.Email)).Returns(true);

            _filter.OnActionExecuting(_filterContext);

            Assert.That(_filterContext.Result, Is.Null);
        }

        [Test]
        public void ThenIShouldNotBeShownThePageIfTheFeatureIsNotEnabledAndIAmLoggedIn()
        {
            _featureToggleService.Setup(f => f.IsFeatureEnabled(ControllerName, ActionName, _currentUser.ExternalUserId, _currentUser.Email)).Returns(false);

            _filter.OnActionExecuting(_filterContext);

            var result = _filterContext.Result as ViewResult;

            Assert.That(result, Is.Not.Null);
            Assert.That(result.ViewName, Is.EqualTo(ControllerConstants.FeatureNotEnabledViewName));
        }

        [Test]
        public void ThenIShouldBeShownThePageIfTheFeatureIsEnabledAndIAmNotLoggedIn()
        {
            _currentUserService.Setup(c => c.GetCurrentUser()).Returns<CurrentUser>(null);
            _featureToggleService.Setup(f => f.IsFeatureEnabled(ControllerName, ActionName, null, null)).Returns(true);

            _filter.OnActionExecuting(_filterContext);

            Assert.That(_filterContext.Result, Is.Null);
        }

        [Test]
        public void ThenIShouldNotBeShownThePageIfTheFeatureIsNotEnabledAndIAmNotLoggedIn()
        {
            _currentUserService.Setup(c => c.GetCurrentUser()).Returns<CurrentUser>(null);
            _featureToggleService.Setup(f => f.IsFeatureEnabled(ControllerName, ActionName, null, null)).Returns(false);
            _currentUserService.Setup(c => c.GetCurrentUser()).Returns(_currentUser);

            _filter.OnActionExecuting(_filterContext);

            var result = _filterContext.Result as ViewResult;

            Assert.That(result, Is.Not.Null);
            Assert.That(result.ViewName, Is.EqualTo(ControllerConstants.FeatureNotEnabledViewName));
        }
    }
}