using System;
using System.Web.Mvc;
using Moq;
using NUnit.Framework;
using SFA.DAS.Authorization;
using SFA.DAS.Authorization.Mvc;

namespace SFA.DAS.UnitTests.Authorization.Mvc
{
    [TestFixture]
    public class ValidateMembershipAttributeTests
    {
        private ValidateMembershipAttribute _filter;
        private Mock<IAuthorizationService> _authorizationService;
        private ActionExecutingContext _filterContext;
        
        [SetUp]
        public void Arrange()
        {
            _authorizationService = new Mock<IAuthorizationService>();
            _filterContext = new ActionExecutingContext();

            _filter = new ValidateMembershipAttribute(() => _authorizationService.Object);
        }

        [Test]
        public void WhenIViewAPageThenIShouldBeShownThePageIfAccountMembershipIsValid()
        {
            _filter.OnActionExecuting(_filterContext);

            Assert.That(_filterContext.Result, Is.Null);
        }

        [Test]
        public void WhenIViewAPageThenIShouldNotBeShownThePageIfAccountMembershipIsValid()
        {
            _authorizationService.Setup(m => m.ValidateMembership()).Throws<UnauthorizedAccessException>();
            
            Assert.Throws<UnauthorizedAccessException>(() => _filter.OnActionExecuting(_filterContext));
        }
    }
}