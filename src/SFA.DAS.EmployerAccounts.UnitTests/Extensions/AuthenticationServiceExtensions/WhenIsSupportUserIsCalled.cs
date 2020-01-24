using Moq;
using NUnit.Framework;
using SFA.DAS.Authentication;
using System.Security.Claims;

namespace SFA.DAS.EmployerAccounts.UnitTests.Extensions.AuthenticationServiceExtensions
{
    public class WhenIsSupportUserIsCalled
    {
        private Mock<IAuthenticationService> _mockAuthenticationService;
        
        [SetUp]
        public void Arrange()
        {
            _mockAuthenticationService = new Mock<IAuthenticationService>();
        }

        [Test]
        public void ThenTheAuthenticationServiceIsCalled()
        {

            //Act
            var result = EmployerAccounts.Extensions.AuthenticationServiceExtensions.IsSupportConsoleUser(_mockAuthenticationService.Object,It.IsAny<string>());

            //Assert
            _mockAuthenticationService.Verify(m => m.HasClaim(ClaimsIdentity.DefaultRoleClaimType, "Tier2User"), Times.Once);
        }

        [Test]
        public void ThenTrueIsReturnedWhenTheAuthenticationServiceHasTheSupportUserClaim()
        {
            // Arrange
            _mockAuthenticationService
                .Setup(m => m.HasClaim(ClaimsIdentity.DefaultRoleClaimType, "Tier2User"))
                .Returns(true);

            //Act
            var result = EmployerAccounts.Extensions.AuthenticationServiceExtensions.IsSupportConsoleUser(_mockAuthenticationService.Object, It.IsAny<string>());

            //Assert
            Assert.IsTrue(result);
        }

        [Test]
        public void ThenFalseIsReturnedWhenTheAuthenticationServiceDoesNotHaveTheSupportUserClaim()
        {
            // Arrange
            _mockAuthenticationService
                .Setup(m => m.HasClaim(ClaimsIdentity.DefaultRoleClaimType, "Tier2User"))
                .Returns(false);

            //Act
            var result = EmployerAccounts.Extensions.AuthenticationServiceExtensions.IsSupportConsoleUser(_mockAuthenticationService.Object, It.IsAny<string>());

            //Assert
            Assert.IsFalse(result);
        }
    }
}
