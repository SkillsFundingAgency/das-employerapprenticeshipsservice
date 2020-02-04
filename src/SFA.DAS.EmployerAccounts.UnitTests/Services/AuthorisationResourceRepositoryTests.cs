using FluentAssertions;
using NUnit.Framework;
using SFA.DAS.EmployerAccounts.Services;
using System.Linq;
using System.Security.Claims;

namespace SFA.DAS.EmployerAccounts.UnitTests.Services
{
    [TestFixture]
    public class AuthorisationResourceRepositoryTests
    {
        private const string Tier2User = "Tier2User";
        private AuthorisationResourceRepository authorisationResourceRepository;
        private ClaimsIdentity claimsIdentity;

        [SetUp]
        public void SetUp()
        {
            authorisationResourceRepository = new AuthorisationResourceRepository();
            claimsIdentity = new ClaimsIdentity();
        }        

        [Test]
        public void AuthorisationResourceRepository_WhenTheUserInRoleIsTier2User_ThenAuthorisationResourcesExist()
        {           
            //Act            
            claimsIdentity.AddClaim(new Claim(claimsIdentity.RoleClaimType, Tier2User));
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
