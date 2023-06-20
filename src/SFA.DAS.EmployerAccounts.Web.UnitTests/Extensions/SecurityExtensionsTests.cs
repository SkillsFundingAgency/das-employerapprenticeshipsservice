using System.Security.Claims;
using System.Security.Principal;
using SFA.DAS.EmployerAccounts.Web.Extensions;
using SFA.DAS.EmployerAccounts.Web.RouteValues;

namespace SFA.DAS.EmployerAccounts.Web.UnitTests.Extensions
{
    [TestFixture]
    public class SecurityExtensionsTests
    {
        private const string Tier2User = "Tier2User";
        private const string HashedAccountId = "HashedAccountId";
        private Mock<IPrincipal> MockIPrincipal;
        private Mock<ClaimsIdentity> mockClaimsIdentity;
        private readonly List<Claim> claims = new List<Claim>();

        [SetUp]
        public void Arrange()
        {           
            MockIPrincipal = new Mock<IPrincipal>();
            mockClaimsIdentity = new Mock<ClaimsIdentity>();            
            MockIPrincipal.Setup(m => m.Identity).Returns(mockClaimsIdentity.Object);
            MockIPrincipal.Setup(x => x.IsInRole(Tier2User)).Returns(true);           
        }


        [Test]
        public void HashedAccountId_WhenClaimsSetWithHashedAccountId_ThenReturnHashedAccountId()
        {
            //Arrange
            claims.Add(new Claim(RouteValueKeys.HashedAccountId, HashedAccountId));
            mockClaimsIdentity.Setup(m => m.Claims).Returns(claims);

            //Act
            var result = SecurityExtensions.HashedAccountId(mockClaimsIdentity.Object);

            //Assert            
            Assert.AreEqual(HashedAccountId, result);
        }

        [Test]
        public void HashedAccountId_WhenClaimsNotSetWithHashedAccountId_ThenReturnHashedAccountIdAsEmptyString()
        {
            //Arrange
            List<Claim> claims = new List<Claim>();
            mockClaimsIdentity.Setup(m => m.Claims).Returns(claims);

            //Act
            var result = SecurityExtensions.HashedAccountId(mockClaimsIdentity.Object);

            //Assert            
            Assert.AreEqual(string.Empty, result);

        }

    }
}
