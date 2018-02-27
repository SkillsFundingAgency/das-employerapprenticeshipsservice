using System;
using System.Web.Mvc;
using Moq;
using NUnit.Framework;
using SFA.DAS.EAS.Web.Attributes;
using SFA.DAS.EAS.Web.Authorization;

namespace SFA.DAS.EAS.Web.UnitTests.Attributes.ValidateMembershipAttributeTests
{
    [TestFixture]
    public class WhenIViewAPage
    {
        private ValidateMembershipAttribute _filter;
        private Mock<IMembershipService> _membershipService;
        private ActionExecutingContext _filterContext;
        
        [SetUp]
        public void Arrange()
        {
            _membershipService = new Mock<IMembershipService>();
            _filterContext = new ActionExecutingContext();

            _filter = new ValidateMembershipAttribute(() => _membershipService.Object);
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
            _membershipService.Setup(m => m.ValidateMembership()).Throws<UnauthorizedAccessException>();
            
            Assert.Throws<UnauthorizedAccessException>(() => _filter.OnActionExecuting(_filterContext));
        }
    }
}