using System;
using System.Web.Mvc;
using System.Web.Routing;
using Moq;
using NUnit.Framework;
using SFA.DAS.EAS.Application.Services;
using SFA.DAS.EAS.Web.Attributes;
using SFA.DAS.EAS.Web.Authentication;
using SFA.DAS.EAS.Web.Helpers;

namespace SFA.DAS.EAS.Web.UnitTests.Attributes.ValidateAccountMembershipAttributeTests
{
    [TestFixture]
    public class WhenIViewAPage
    {
        private const string AccountHashedId = "ABC123";
        private static readonly Guid UserExternalId = Guid.NewGuid();

        private ValidateAccountMembershipAttribute _filter;
        private Mock<ICurrentUserService> _currentUserService;
        private Mock<IMembershipService> _membershipService;
        private CurrentUser _currentUser;
        private ActionExecutingContext _filterContext;
        private RouteData _routeData;
        
        [SetUp]
        public void Arrange()
        {
            _currentUserService = new Mock<ICurrentUserService>();
            _membershipService = new Mock<IMembershipService>();
            _routeData = new RouteData();

            _routeData.Values.Add(ControllerConstants.HashedAccountIdKeyName, AccountHashedId);

            _filterContext = new ActionExecutingContext { RouteData = _routeData };
            _currentUser = new CurrentUser { ExternalId = UserExternalId };

            _currentUserService.Setup(c => c.GetCurrentUser()).Returns(_currentUser);

            _filter = new ValidateAccountMembershipAttribute(() => _currentUserService.Object, () => _membershipService.Object);
        }

        [Test]
        public void ThenIShouldBeShownThePageIfAccountMembershipIsValid()
        {
            _filter.OnActionExecuting(_filterContext);

            Assert.That(_filterContext.Result, Is.Null);
        }

        [Test]
        public void ThenIShouldNotBeShownThePageIfAccountMembershipIsValid()
        {
            _membershipService.Setup(m => m.ValidateAccountMembership(AccountHashedId, UserExternalId)).Throws<UnauthorizedAccessException>();
            
            Assert.Throws<UnauthorizedAccessException>(() => _filter.OnActionExecuting(_filterContext));
        }
    }
}