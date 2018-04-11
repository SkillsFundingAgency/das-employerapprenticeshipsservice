using System;
using System.Web.Mvc;
using Moq;
using NUnit.Framework;
using SFA.DAS.EAS.Domain.Interfaces;
using SFA.DAS.EAS.Infrastructure.Authorization;
using SFA.DAS.EAS.Web.Attributes;

namespace SFA.DAS.EAS.Web.UnitTests.Attributes.ValidateMembershipAttributeTests
{
    [TestFixture]
    public class WhenIViewAPage
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
        public void ThenIShouldBeShownThePageIfAccountMembershipIsValid()
        {
            _filter.OnActionExecuting(_filterContext);

            Assert.That(_filterContext.Result, Is.Null);
        }

        [Test]
        public void ThenIShouldNotBeShownThePageIfAccountMembershipIsValid()
        {
            _authorizationService.Setup(m => m.ValidateMembership()).Throws<UnauthorizedAccessException>();
            
            Assert.Throws<UnauthorizedAccessException>(() => _filter.OnActionExecuting(_filterContext));
        }
    }
}