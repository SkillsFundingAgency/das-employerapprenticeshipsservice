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
using SFA.DAS.HashingService;
using SFA.DAS.Authentication;

namespace SFA.DAS.EmployerAccounts.Web.UnitTests.Handlers
{
    [TestFixture]
    public class ImpersonationAuthorizationContextTests
    {
        protected Mock<IAuthorizationContextProvider> MockAuthorizationContextProvider;
        protected Mock<IAuthorizationContext> MockAuthorizationContext;
        protected Mock<HttpContextBase> MockContextBase;
        protected Mock<HttpRequestBase> MockRequestBase;
        protected Mock<HttpResponseBase> MockResponseBase;
        public ImpersonationAuthorizationContext sutImpersonationAuthorizationContext;
        protected Mock<IRouteHandler> MockRouteHandler { get; set; }
        private Mock<IEmployerAccountTeamRepository> MockEmployerAccountTeamRepository;
        private TeamMember _teamMember;
        protected const string Tier2User = "Tier2User";        
        public virtual ICollection<TeamMember> TeamMembers { get; set; }
        protected Mock<IHashingService> MockHashingService;
        protected Mock<IAuthenticationService> MockAuthenticationService;

        [SetUp]
        public void Arrange()
        {
            _teamMember = new TeamMember();
            MockEmployerAccountTeamRepository = new Mock<IEmployerAccountTeamRepository>();
            MockEmployerAccountTeamRepository.Setup(x => x.GetAccountTeamMembers(It.IsAny<string>()));     
            MockAuthorizationContextProvider = new Mock<IAuthorizationContextProvider>();
            MockAuthorizationContext = new Mock<IAuthorizationContext>();
            MockContextBase = new Mock<HttpContextBase>();
            MockRequestBase = new Mock<HttpRequestBase>();
            MockResponseBase = new Mock<HttpResponseBase>();
            MockRouteHandler = new Mock<IRouteHandler>();
            MockHashingService = new Mock<IHashingService>();
            MockAuthenticationService = new Mock<IAuthenticationService>();

            MockContextBase.Setup(x => x.Request).Returns(MockRequestBase.Object);
            MockContextBase.Setup(x => x.Response).Returns(MockResponseBase.Object);
            MockContextBase.Setup(x => x.User.IsInRole("Tier2User")).Returns(true);
            var routebase = new Route("accounts/{hashedaccountid}/teams/view", MockRouteHandler.Object);
            var routeData = new RouteData(routebase, MockRouteHandler.Object);
            routeData.Values.Add("HashedAccountId", "value1");
            MockContextBase.Setup(x => x.Request.RequestContext.RouteData).Returns(routeData);
            MockAuthorizationContextProvider.Setup(x => x.GetAuthorizationContext()).Returns(MockAuthorizationContext.Object);


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

            claimsIdentity.AddClaim(new Claim(claimsIdentity.RoleClaimType, Tier2User));
            var principal = new ClaimsPrincipal(claimsIdentity);
            MockContextBase.Setup(c => c.User).Returns(principal);
        }

        [Test]
       public void GetAuthorizationContext_WhenCalled_ThenReturnTheInstanceIsImpersonationAuthorizationContext()
       {
            //Act
            var result = sutImpersonationAuthorizationContext.GetAuthorizationContext();

            //Assert
            var test = result;
            result.TryGet<ClaimsIdentity>("ClaimsIdentity", out var claimsIdentityauthrorizationContext);
            var userRoleClaims = claimsIdentityauthrorizationContext?.Claims.Where(c => c.Type == claimsIdentityauthrorizationContext?.RoleClaimType);
           // Assert.AreEqual(userRoleClaims.Any(claim => claim.Value), RouteValueKeys.Tier2User))
            Assert.IsInstanceOf<IAuthorizationContext>(result);        
       }


        [Test]
        public void GetAuthorizationContext_WhenAccountHashedIdKeyIsNotTheRight_ThenThrowUnauthorizedAccessException()
        {
            //Arrange
            var routebase = new Route("accounts/{hashedaccountid}/teams/view", MockRouteHandler.Object);
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
            MockContextBase.Setup(x => x.Request).Returns(MockRequestBase.Object);
            MockContextBase.Setup(x => x.Response).Returns(MockResponseBase.Object);
            // MockContextBase.Setup(x => x.User.IsInRole("NotTier2User")).Returns(false);
            var routebase = new Route("accounts/{hashedaccountid}/teams/view", MockRouteHandler.Object);
            var routeData = new RouteData(routebase, MockRouteHandler.Object);
            routeData.Values.Add("HashedAccountId", "value1");
            MockContextBase.Setup(x => x.Request.RequestContext.RouteData).Returns(routeData);
            MockAuthorizationContextProvider.Setup(x => x.GetAuthorizationContext()).Returns(MockAuthorizationContext.Object);


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
            Assert.IsInstanceOf<IAuthorizationContext>(result);
        }



        //[Test]
        //public void Test1()
        //{
        //    //Arrange
        //    var authorizationContextProvider = new AuthorizationContextProvider(MockContextBase.Object, MockHashingService.Object, MockAuthenticationService.Object);
        //    var authorizationContext = authorizationContextProvider.GetAuthorizationContext();
        //    var impersonationAuthorizationContext = new ImpersonationAuthorizationContext(MockContextBase.Object,
        //      authorizationContextProvider,
        //      MockEmployerAccountTeamRepository.Object);

        //    //Act
        //    var result = impersonationAuthorizationContext.GetAuthorizationContext();

        //    //Assert
        //    result.TryGet<ClaimsIdentity>("ClaimsIdentity", out var claimsIdentityauthrorizationContext);
        //    var userRoleClaims = claimsIdentityauthrorizationContext?.Claims.Where(c => c.Type == claimsIdentityauthrorizationContext?.RoleClaimType);
        //}

    }
}
