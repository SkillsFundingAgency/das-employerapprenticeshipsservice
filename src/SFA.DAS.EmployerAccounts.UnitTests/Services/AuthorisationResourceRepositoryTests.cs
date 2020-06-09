using FluentAssertions;
using NUnit.Framework;
using SFA.DAS.EmployerAccounts.Services;
using System.Linq;
using System.Security.Claims;
using Moq;
using SFA.DAS.Authentication;
using SFA.DAS.EmployerAccounts.Configuration;
using SFA.DAS.EmployerAccounts.Extensions;

namespace SFA.DAS.EmployerAccounts.UnitTests.Services
{
    [TestFixture]
    public class AuthorisationResourceRepositoryTests
    {
        private AuthorisationResourceRepository _authorisationResourceRepository;
        private ClaimsIdentity _claimsIdentity;
        private IUserContext _userContext;
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
            _userContext =new UserContext(_mockAuthenticationService.Object,_config);
            _authorisationResourceRepository = new AuthorisationResourceRepository(_userContext);
            _claimsIdentity = new ClaimsIdentity();
        }        

        [Test]
        [TestCase("Tier1User")]
        [TestCase("Tier2User")]
        public void AuthorisationResourceRepository_WhenTheUserInRoleIsTier2User_ThenAuthorisationResourcesExist(string role)
        {
            //Arrange
            _mockAuthenticationService.Setup(m => m.HasClaim(ClaimsIdentity.DefaultRoleClaimType, role)).Returns(true);

            //Act            
            _claimsIdentity.AddClaim(new Claim(_claimsIdentity.RoleClaimType, role));
            var result = _authorisationResourceRepository.Get(_claimsIdentity);

            //Assert
            result.Count().Should().BeGreaterThan(0);
        }

        [Test]
        public void AuthorisationResourceRepository_WhenTheUserInRoleIsNotTier2User_ThenAuthorisationResourcesDoNotExist()
        {
            //Act
            var result = _authorisationResourceRepository.Get(_claimsIdentity);

            //Assert
            result.Count().Should().Be(0);
        }
    }
}
