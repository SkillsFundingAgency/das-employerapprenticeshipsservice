using Moq;
using NUnit.Framework;
using SFA.DAS.Authorization.Context;
using SFA.DAS.EmployerAccounts.Data;
using SFA.DAS.EmployerAccounts.Web.Authorization;
using SFA.DAS.EmployerUsers.WebClientComponents;
using SFA.DAS.EmployerAccounts.Models.AccountTeam;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using System.Web.Routing;
using SFA.DAS.EmployerAccounts.Models;
using System;
using System.Linq;
using SFA.DAS.Authentication;
using SFA.DAS.EmployerAccounts.Configuration;
using static SFA.DAS.EmployerAccounts.Web.Authorization.ImpersonationAuthorizationContext;

namespace SFA.DAS.EmployerAccounts.Web.UnitTests.Authorization
{
    [TestFixture]
    public class ImpersonationAuthorizationContextTests
    {
        protected Mock<IAuthorizationContextProvider> MockAuthorizationContextProvider;
        protected Mock<HttpContextBase> MockContextBase;
        public ImpersonationAuthorizationContext SutImpersonationAuthorizationContext;
        protected Mock<IRouteHandler> MockRouteHandler { get; set; }
        private Mock<IEmployerAccountTeamRepository> MockEmployerAccountTeamRepository;
        private TeamMember _teamMember;
        private ClaimsIdentity claimsIdentity;
        public virtual ICollection<TeamMember> TeamMembers { get; set; }
        private EmployerAccountsConfiguration _configuration;
        private Mock<IAuthenticationService> _mockAuthenticationService;
        private readonly string SupportConsoleUsers = "Tier1User,Tier2User";


        [SetUp]
        public void Arrange()
        {
            _teamMember = new TeamMember
            {
                AccountId = 123,
                Role = Role.Owner,
                UserRef = Guid.NewGuid(),
                Email = "vas@test.com"
            };

            _configuration = new EmployerAccountsConfiguration
            {
                SupportConsoleUsers = SupportConsoleUsers
            };

            _mockAuthenticationService = new Mock<IAuthenticationService>();
            MockEmployerAccountTeamRepository = new Mock<IEmployerAccountTeamRepository>();
            MockEmployerAccountTeamRepository.Setup(x => x.GetAccountTeamMembers(It.IsAny<string>()));
            MockAuthorizationContextProvider = new Mock<IAuthorizationContextProvider>();
            MockContextBase = new Mock<HttpContextBase>();
            MockRouteHandler = new Mock<IRouteHandler>();

            var routeBase = new Route("teams/view", MockRouteHandler.Object);
            var routeData = new RouteData(routeBase, MockRouteHandler.Object);
            routeData.Values.Add(RouteValueKeys.AccountHashedId, "ABC123");
            MockContextBase.Setup(x => x.Request.RequestContext.RouteData).Returns(routeData);

            var authorizationContext = new DAS.Authorization.Context.AuthorizationContext();
            MockAuthorizationContextProvider.Setup(x => x.GetAuthorizationContext()).Returns(authorizationContext);

            TeamMembers = new List<TeamMember> { _teamMember };
            MockEmployerAccountTeamRepository.Setup(x => x.GetAccountTeamMembers(It.IsAny<string>())).Returns(Task.FromResult(TeamMembers));

            SutImpersonationAuthorizationContext = new ImpersonationAuthorizationContext
              (MockContextBase.Object,
              MockAuthorizationContextProvider.Object,
              MockEmployerAccountTeamRepository.Object,
              _configuration,
              _mockAuthenticationService.Object);

            claimsIdentity = new ClaimsIdentity(new[]
            {
                new Claim(DasClaimTypes.Id, "UserRef"),
                new Claim(DasClaimTypes.Email, "Email"),
                new Claim("sub", "UserRef"),
            });
        }

        [Test]
        [TestCase("Tier1User")]
        [TestCase("Tier2User")]
        public void GetAuthorizationContext_WhenClaimsIdentityAndResourceBeenSet_ThenReturnClaimsIdentityAndResource(string role)
        {
            //Act
            
            _mockAuthenticationService.Setup(m => m.HasClaim(ClaimsIdentity.DefaultRoleClaimType, role)).Returns(true);
            claimsIdentity.AddClaim(new Claim(claimsIdentity.RoleClaimType, role));
            var principal = new ClaimsPrincipal(claimsIdentity);
            MockContextBase.Setup(c => c.User).Returns(principal);

            var result = SutImpersonationAuthorizationContext.GetAuthorizationContext();
            
            //Assert            
            result.TryGet<ClaimsIdentity>("ClaimsIdentity", out var claimsIdentityauthrorizationContext);
            var userRoleClaims = claimsIdentityauthrorizationContext?.Claims.Where(c => c.Type == claimsIdentityauthrorizationContext.RoleClaimType);
            Assert.IsTrue(userRoleClaims.Any(claim => claim.Value == role));

            result.TryGet<Resource>("Resource", out var resource);
            var resourceValue = resource != null ? resource.Value : "default";
            Assert.AreEqual("teams/view", resourceValue);

            Assert.IsInstanceOf<IAuthorizationContext>(result);
        }


        [Test]
        public void GetAuthorizationContext_WhenAccountHashedIdKeyIsNotRight_ThenThrowUnauthorizedAccessException()
        {
            //Arrange       
            var routeBase = new Route("teams/view", MockRouteHandler.Object);
            var routeData = new RouteData(routeBase, MockRouteHandler.Object);
            routeData.Values.Add("HashedAccountId123", "value1");
            
            //Act
            try
            {
                var result = SutImpersonationAuthorizationContext.GetAuthorizationContext();
            }
            catch (UnauthorizedAccessException ex)
            {
                //Assert             
                Assert.AreEqual(ex.Message, "Attempted to perform an unauthorized operation.");
            }
        }

        [Test]
        public void GetAuthorizationContext_WhenRoleClaimTypeIsNotSet_ThenDoNotReturnClaimsIdentityAndResource()
        {
            //Arrange        
            var claimsIdentity = new ClaimsIdentity(new[]
            {
                new Claim(DasClaimTypes.Id, "UserRef"),
                new Claim(DasClaimTypes.Email, "Email"),
                new Claim("sub", "UserRef"),
            });
            
            var principal = new ClaimsPrincipal(claimsIdentity);
            MockContextBase.Setup(c => c.User).Returns(principal);

            //Act
            var result = SutImpersonationAuthorizationContext.GetAuthorizationContext();

            //Assert            
            result.TryGet<ClaimsIdentity>("ClaimsIdentity", out var claimsIdentityauthrorizationContext);
            Assert.IsNull(claimsIdentityauthrorizationContext);

            result.TryGet<Resource>("Resource", out var resource);
            Assert.IsNull(resource);

            Assert.IsInstanceOf<IAuthorizationContext>(result);
        }

    }
}
