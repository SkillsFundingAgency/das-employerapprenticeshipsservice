using System.Linq;
using System.Security.Claims;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Moq;
using NUnit.Framework;
using SFA.DAS.EmployerAccounts.Configuration;
using SFA.DAS.EmployerAccounts.Extensions;
using SFA.DAS.EmployerAccounts.Services;

namespace SFA.DAS.EmployerAccounts.UnitTests.Services
{
    [TestFixture]
    public class AuthorisationResourceRepositoryTests
    {
        private AuthorisationResourceRepository _authorisationResourceRepository;
        private ClaimsIdentity _claimsIdentity;
        private IUserContext _userContext;
        private EmployerAccountsConfiguration _config;
        private Mock<IHttpContextAccessor> _httpContextAccessorMock;
        private readonly string SupportConsoleUsers = "Tier1User,Tier2User";

        [SetUp]
        public void SetUp()
        {
            _config = new EmployerAccountsConfiguration()
            {
                SupportConsoleUsers = SupportConsoleUsers
            };
            _httpContextAccessorMock = new Mock<IHttpContextAccessor>();
            _userContext =new UserContext(_httpContextAccessorMock.Object,_config);
            _authorisationResourceRepository = new AuthorisationResourceRepository(_userContext);
            _claimsIdentity = new ClaimsIdentity();
        }        

        [Test]
        [TestCase("Tier1User")]
        [TestCase("Tier2User")]
        public void AuthorisationResourceRepository_WhenTheUserInRoleIsTier2User_ThenAuthorisationResourcesExist(string role)
        {
            //Arrange
            _httpContextAccessorMock.Setup(m => m.HttpContext.User.HasClaim(ClaimsIdentity.DefaultRoleClaimType, role)).Returns(true);

            //Act            
            _claimsIdentity.AddClaim(new Claim(_claimsIdentity.RoleClaimType, role));
            var result = _authorisationResourceRepository.Get(_claimsIdentity);

            //Assert
            result.Count().Should().BeGreaterThan(0);
        }

        [Test]
        public void AuthorisationResourceRepository_WhenTheUserInRoleIsNotTier2User_ThenAuthorisationResourcesDoNotExist()
        {
            //Arrange
            _httpContextAccessorMock.Setup(m => m.HttpContext.User.HasClaim(ClaimsIdentity.DefaultRoleClaimType, It.IsAny<string>())).Returns(false);

            //Act
            var result = _authorisationResourceRepository.Get(_claimsIdentity);

            //Assert
            result.Count().Should().Be(0);
        }
    }
}
