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
using static SFA.DAS.EmployerAccounts.Web.Authorization.ImpersonationAuthorizationContext;

namespace SFA.DAS.EmployerAccounts.Web.UnitTests.Handlers
{
    [TestFixture]
    public class ImpersonationAuthorizationContextTests
    {
        protected Mock<IAuthorizationContextProvider> MockAuthorizationContextProvider;        
        protected Mock<HttpContextBase> MockContextBase;       
        public ImpersonationAuthorizationContext sutImpersonationAuthorizationContext;
        protected Mock<IRouteHandler> MockRouteHandler { get; set; }
        private Mock<IEmployerAccountTeamRepository> MockEmployerAccountTeamRepository;
        private TeamMember _teamMember;               
        public virtual ICollection<TeamMember> TeamMembers { get; set; }   

        [SetUp]
        public void Arrange()
        {
            _teamMember = new TeamMember
            {
                AccountId = 123,
                Role = Role.Owner,
                UserRef = "UserRef",
                Email = "vas@test.com"
            };
            MockEmployerAccountTeamRepository = new Mock<IEmployerAccountTeamRepository>();
            MockEmployerAccountTeamRepository.Setup(x => x.GetAccountTeamMembers(It.IsAny<string>()));     
            MockAuthorizationContextProvider = new Mock<IAuthorizationContextProvider>();
            MockContextBase = new Mock<HttpContextBase>();            
            MockRouteHandler = new Mock<IRouteHandler>();            
            
            MockContextBase.Setup(x => x.User.IsInRole(AuthorizationConstants.Tier2User)).Returns(true);
            var routebase = new Route(AuthorizationConstants.TeamViewRoute, MockRouteHandler.Object);
            var routeData = new RouteData(routebase, MockRouteHandler.Object);
            routeData.Values.Add(RouteValueKeys.AccountHashedId, "ABC123");
            MockContextBase.Setup(x => x.Request.RequestContext.RouteData).Returns(routeData);

            var authorizationContext = new SFA.DAS.Authorization.Context.AuthorizationContext();
            MockAuthorizationContextProvider.Setup(x => x.GetAuthorizationContext()).Returns(authorizationContext);

            TeamMembers = new List<TeamMember> { _teamMember };          
            MockEmployerAccountTeamRepository.Setup(x => x.GetAccountTeamMembers(It.IsAny<string>())).Returns(Task.FromResult(TeamMembers));

            sutImpersonationAuthorizationContext = new ImpersonationAuthorizationContext
              (MockContextBase.Object,
              MockAuthorizationContextProvider.Object,
              MockEmployerAccountTeamRepository.Object);

            var claimsIdentity = new ClaimsIdentity(new[]
           {
                new Claim(DasClaimTypes.Id, "UserRef"),
                new Claim(DasClaimTypes.Email, "Email"),
                new Claim("sub", "UserRef"),
            });

            claimsIdentity.AddClaim(new Claim(claimsIdentity.RoleClaimType, AuthorizationConstants.Tier2User));
            var principal = new ClaimsPrincipal(claimsIdentity);
            MockContextBase.Setup(c => c.User).Returns(principal);
        }

        [Test]
       public void GetAuthorizationContext_WhenCalled_ThenReturnTheInstanceIsImpersonationAuthorizationContext()
       {
            //Act
            var result = sutImpersonationAuthorizationContext.GetAuthorizationContext();

            //Assert            
            result.TryGet<ClaimsIdentity>("ClaimsIdentity", out var claimsIdentityauthrorizationContext);
            var userRoleClaims = claimsIdentityauthrorizationContext?.Claims.Where(c => c.Type == claimsIdentityauthrorizationContext?.RoleClaimType);            
            Assert.IsTrue(userRoleClaims.Any(claim => claim.Value == AuthorizationConstants.Tier2User));            

            result.TryGet<Resource>("Resource", out var resource);
            var resourceValue = resource != null ? resource.Value : "default";
            Assert.AreEqual(AuthorizationConstants.TeamViewRoute, resourceValue);

            Assert.IsInstanceOf<IAuthorizationContext>(result);        
       }


        [Test]
        public void GetAuthorizationContext_WhenAccountHashedIdKeyIsNotTheRight_ThenThrowUnauthorizedAccessException()
        {
            //Arrange
            var routebase = new Route(AuthorizationConstants.TeamViewRoute, MockRouteHandler.Object);
            var routeData = new RouteData(routebase, MockRouteHandler.Object);
            routeData.Values.Add("HashedAccountId123", "value1");
         
            //Act
            try
            {
                var result = sutImpersonationAuthorizationContext.GetAuthorizationContext();
            }
            catch (UnauthorizedAccessException ex)
            {
                //Assert             
                Assert.AreEqual(ex.Message, "Attempted to perform an unauthorized operation.");
            }
        }

        [Test]
        public void GetAuthorizationContext_WhenRoleClaimTypeIsNotSet_ThenReturnAuthorizationContext()
        {
            //Arrange                       
            // MockContextBase.Setup(x => x.User.IsInRole("NotTier2User")).Returns(false);
            var routebase = new Route("accounts/{hashedaccountid}/teams/view", MockRouteHandler.Object);
            var routeData = new RouteData(routebase, MockRouteHandler.Object);
            routeData.Values.Add("HashedAccountId", "value1");
            MockContextBase.Setup(x => x.Request.RequestContext.RouteData).Returns(routeData);
            //MockAuthorizationContextProvider.Setup(x => x.GetAuthorizationContext()).Returns(MockAuthorizationContext.Object);

            var authorizationContext = new SFA.DAS.Authorization.Context.AuthorizationContext();
            MockAuthorizationContextProvider.Setup(x => x.GetAuthorizationContext()).Returns(authorizationContext);

            TeamMembers = new List<TeamMember> { _teamMember };
            _teamMember.AccountId = 123;
            _teamMember.Role = Role.Owner;
            _teamMember.UserRef = "UserRef";
            _teamMember.Email = "vt@test.com";
            MockEmployerAccountTeamRepository.Setup(x => x.GetAccountTeamMembers(It.IsAny<string>())).Returns(Task.FromResult(TeamMembers));

            sutImpersonationAuthorizationContext = new ImpersonationAuthorizationContext
              (MockContextBase.Object,
              MockAuthorizationContextProvider.Object,
              MockEmployerAccountTeamRepository.Object);

            var claimsIdentity = new ClaimsIdentity(new[]
           {
                new Claim(DasClaimTypes.Id, "UserRef"),
                new Claim(DasClaimTypes.Email, "Email"),
                new Claim("sub", "UserRef"),
            });

            //  claimsIdentity.AddClaim(new Claim(claimsIdentity.RoleClaimType, Tier2User));
            var principal = new ClaimsPrincipal(claimsIdentity);
            MockContextBase.Setup(c => c.User).Returns(principal);


            // claimsIdentity.RemoveClaim(new Claim(claimsIdentity.RoleClaimType, Tier2User));

            //Act
            var result = sutImpersonationAuthorizationContext.GetAuthorizationContext();

            //Assert

            var test = result;
            result.TryGet<ClaimsIdentity>("ClaimsIdentity", out var claimsIdentityauthrorizationContext);          
            Assert.IsNull(claimsIdentityauthrorizationContext);

            result.TryGet<Resource>("Resource", out var resource);
            Assert.IsNull(resource);
            
            Assert.IsInstanceOf<IAuthorizationContext>(result);
        }       

    }
}
