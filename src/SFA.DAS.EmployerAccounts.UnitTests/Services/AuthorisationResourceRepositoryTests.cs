using FluentAssertions;
using NUnit.Framework;
using SFA.DAS.EmployerAccounts.Services;
using System.Linq;
using System.Security.Claims;
using Moq;
using SFA.DAS.Authentication;
using SFA.DAS.EmployerAccounts.Configuration;

namespace SFA.DAS.EmployerAccounts.UnitTests.Services
{
    [TestFixture]
    public class AuthorisationResourceRepositoryTests
    {
        private AuthorisationResourceRepository authorisationResourceRepository;
        private ClaimsIdentity claimsIdentity;
        private EmployerAccountsConfiguration _config;
        private Mock<IAuthenticationService> _mockAuthenticationService;
        private readonly string SupportConsoleUsers = "Tier1User,Tier2User";

        [SetUp]
        public void SetUp()
        {

            _config = new EmployerAccountsConfiguration()
            {
                SupportConsoleUsers = SupportConsoleUsers
            };
            _mockAuthenticationService = new Mock<IAuthenticationService>();
            authorisationResourceRepository = new AuthorisationResourceRepository(_mockAuthenticationService.Object, _config);
            claimsIdentity = new ClaimsIdentity();
        }        

        [Test]
        [TestCase("Tier1User")]
        [TestCase("Tier2User")]
        public void AuthorisationResourceRepository_WhenTheUserInRoleIsTier2User_ThenAuthorisationResourcesExist(string role)
        {
            //Arrange
            _mockAuthenticationService.Setup(m => m.HasClaim(ClaimsIdentity.DefaultRoleClaimType, role)).Returns(true);

            //Act            
            claimsIdentity.AddClaim(new Claim(claimsIdentity.RoleClaimType, role));
            var result = authorisationResourceRepository.Get(claimsIdentity);

            //Assert
            result.Count().Should().BeGreaterThan(0);
        }

        [Test]
        public void AuthorisationResourceRepository_WhenTheUserInRoleIsNotTier2User_ThenAuthorisationResourcesDoNotExist()
        {
            //Act
            var result = authorisationResourceRepository.Get(claimsIdentity);

            //Assert
            result.Count().Should().Be(0);
        }
    }
}
