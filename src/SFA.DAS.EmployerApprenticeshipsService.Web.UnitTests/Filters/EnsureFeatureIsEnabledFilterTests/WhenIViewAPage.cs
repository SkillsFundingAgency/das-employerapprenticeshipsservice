using System.Web.Mvc;
using System.Web.Routing;
using Moq;
using NUnit.Framework;
using SFA.DAS.EAS.Domain.Interfaces;
using SFA.DAS.EAS.Domain.Models.Authorization;
using SFA.DAS.EAS.Web.Authorization;
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
        private Mock<IFeatureToggleService> _featureToggleService;
        private Mock<IMembershipService> _membershipService;
        private IMembershipContext _membershipContext;
        
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
            _membershipService = new Mock<IMembershipService>();
            _membershipContext = new MembershipContext();

            _membershipService.Setup(m => m.GetMembershipContext()).Returns(_membershipContext);
            _featureToggleService.Setup(f => f.IsFeatureEnabled(ControllerName, ActionName, _membershipContext)).Returns(true);

            _filter = new ValidateFeatureFilter(() => _featureToggleService.Object, () => _membershipService.Object);
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
            _featureToggleService.Setup(f => f.IsFeatureEnabled(ControllerName, ActionName, _membershipContext)).Returns(false);

            _filter.OnActionExecuting(_filterContext);

            var result = _filterContext.Result as ViewResult;

            Assert.That(result, Is.Not.Null);
            Assert.That(result.ViewName, Is.EqualTo(ControllerConstants.FeatureNotEnabledViewName));
        }
    }
}