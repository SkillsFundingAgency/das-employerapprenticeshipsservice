﻿using Moq;
using NUnit.Framework;
using SFA.DAS.Authentication;
using System.Security.Claims;
using SFA.DAS.EmployerAccounts.Configuration;
using SFA.DAS.EmployerAccounts.Extensions;

namespace SFA.DAS.EmployerAccounts.UnitTests.Extensions.AuthenticationServiceExtensions
{
    public class WhenIsSupportUserIsCalled
    {
        private Mock<IAuthenticationService> _mockAuthenticationService;
        private EmployerAccountsConfiguration _employerAccountsConfiguration;
        private IUserContext _userContext;
        private readonly string _supportConsoleUsers = "Tier1User,Tier2User";

        [SetUp]
        public void Arrange()
        {
            _mockAuthenticationService = new Mock<IAuthenticationService>();
            _employerAccountsConfiguration=new EmployerAccountsConfiguration()
            {
                SupportConsoleUsers = _supportConsoleUsers
            };
            _userContext = new UserContext(_mockAuthenticationService.Object,_employerAccountsConfiguration);
        }

        [Test]
        [TestCase("Tier1User")]
        [TestCase("Tier2User")]
        public void ThenTheAuthenticationServiceIsCalled(string role)
        {
            //Act
            _userContext.IsSupportConsoleUser();

            //Assert
            _mockAuthenticationService.Verify(m => m.HasClaim(ClaimsIdentity.DefaultRoleClaimType, role), Times.Once);
        }

        [Test]
        [TestCase("Tier1User")]
        [TestCase("Tier2User")]
        public void ThenTrueIsReturnedWhenTheAuthenticationServiceHasTheSupportUserClaim(string role)
        {
            // Arrange
            _mockAuthenticationService
                .Setup(m => m.HasClaim(ClaimsIdentity.DefaultRoleClaimType, role))
                .Returns(true);

            //Act
            var result = _userContext.IsSupportConsoleUser();

            //Assert
            Assert.IsTrue(result);
        }

        [Test]
        [TestCase("Tier1User")]
        [TestCase("Tier2User")]
        public void ThenFalseIsReturnedWhenTheAuthenticationServiceDoesNotHaveTheSupportUserClaim(string role)
        {
            // Arrange
            _mockAuthenticationService
                .Setup(m => m.HasClaim(ClaimsIdentity.DefaultRoleClaimType, role))
                .Returns(false);

            //Act
            var result = _userContext.IsSupportConsoleUser();

            //Assert
            Assert.IsFalse(result);
        }
    }
}
