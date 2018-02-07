using System;
using Moq;
using NUnit.Framework;
using SFA.DAS.EAS.Web.Authentication;
using SFA.DAS.EAS.Web.Helpers;

namespace SFA.DAS.EAS.Web.UnitTests.Authentication.CurrentUserServiceTests
{
    [TestFixture]
    public class WhenIGetTheCurrentUser
    {
        private static readonly Guid UserExternalId = Guid.NewGuid();
        private const string UserEmail = "john.doe@ma.local";

        private ICurrentUserService _currentUserService;
        private Mock<IOwinWrapper> _owinService;


        [SetUp]
        public void Arrange()
        {
            _owinService = new Mock<IOwinWrapper>();

            _owinService.Setup(s => s.IsUserAuthenticated()).Returns(true);
            _owinService.Setup(s => s.GetClaimValue(ControllerConstants.SubClaimKeyName)).Returns(UserExternalId.ToString());
            _owinService.Setup(s => s.GetClaimValue(ControllerConstants.EmailClaimKeyName)).Returns(UserEmail);

            _currentUserService = new CurrentUserService(_owinService.Object);
        }

        [Test]
        public void ThenShouldReturnTheCurrentUser()
        {
            var user = _currentUserService.GetCurrentUser();

            Assert.That(user, Is.Not.Null);
            Assert.That(user.ExternalId, Is.EqualTo(UserExternalId));
            Assert.That(user.Email, Is.EqualTo(UserEmail));
        }

        [Test]
        public void ThenShouldReturnNullIfTheCurrentUserIsNotAuthenticated()
        {
            _owinService.Setup(s => s.IsUserAuthenticated()).Returns(false);

            var user = _currentUserService.GetCurrentUser();

            Assert.That(user, Is.Null);
        }
    }
}