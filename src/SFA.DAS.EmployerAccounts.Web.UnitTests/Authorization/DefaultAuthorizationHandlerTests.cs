using Moq;
using NUnit.Framework;
using SFA.DAS.Authorization.Context;
using System.Collections.Generic;
using FluentAssertions;
using System.Threading.Tasks;
using System.Web.Routing;
using System.Web;
using System.Security.Claims;
using SFA.DAS.NLog.Logger;
using SFA.DAS.EmployerAccounts.Web.Authorization;
using AuthorizationContext = SFA.DAS.Authorization.Context.AuthorizationContext;
using SFA.DAS.EmployerUsers.WebClientComponents;
using static SFA.DAS.EmployerAccounts.Web.Authorization.ImpersonationAuthorizationContext;

namespace SFA.DAS.EmployerAccounts.Web.UnitTests.Authorization
{
    [TestFixture]
    public class DefaultAuthorizationHandlerTests
    {
        public List<string> Options { get; set; }
        public IAuthorizationContext AuthorizationContext { get; set; }
        public DefaultAuthorizationHandler SutDefaultAuthorizationHandler { get; set; }
        public AuthorizationContextTestsFixture AuthorizationContextTestsFixture { get; set; }

        [SetUp]
        public void Arrange()
        {
            AuthorizationContextTestsFixture = new AuthorizationContextTestsFixture();
            Options = new List<string>();
            SutDefaultAuthorizationHandler = new DefaultAuthorizationHandler();
            AuthorizationContext = new AuthorizationContext();
        }

        [Test]
        public async Task GetAuthorizationResult_WhenTheUserInRoleIsNotTier2_ThenAuthorizedTheUser()
        {
            //Act            
            var authorizationResult = await SutDefaultAuthorizationHandler.GetAuthorizationResult(Options, AuthorizationContext);

            //Assert
            authorizationResult.IsAuthorized.Should().Be(true);
        }
       

        [Test]
        public void GetAuthorizationResult_WhenTheUserInRoleIsTier2_ThenAllowTheUserToViewTeamPage()
        {
            //Arrange
            AuthorizationContextTestsFixture.SetData(AuthorizationConstants.TeamViewRoute);

            //Act
            AuthorizationContextTestsFixture.AuthorizationContext.ToString();

            //Assert
            var authorizationResult = SutDefaultAuthorizationHandler.GetAuthorizationResult(Options, AuthorizationContextTestsFixture.AuthorizationContext);
            authorizationResult.Result.IsAuthorized.Should().Be(true);
        }

        [Test]
        public void GetAuthorizationResult_WhenTheUserInRoleIsTier2_ThenAllowTheUserToViewTeamInvitePage()
        {
            //Arrange
            AuthorizationContextTestsFixture.SetData(AuthorizationConstants.TeamInvite);

            //Act
            AuthorizationContextTestsFixture.AuthorizationContext.ToString();

            //Assert
            var authorizationResult = SutDefaultAuthorizationHandler.GetAuthorizationResult(Options, AuthorizationContextTestsFixture.AuthorizationContext);
            authorizationResult.Result.IsAuthorized.Should().Be(true);
        }


        [Test]
        public void GetAuthorizationResult_WhenTheUserInRoleIsTier2_ThenAllowTheUserToViewTeamReviewPage()
        {
            //Arrange
            AuthorizationContextTestsFixture.SetData(AuthorizationConstants.TeamReview);

            //Act
            AuthorizationContextTestsFixture.AuthorizationContext.ToString();

            //Assert
            var authorizationResult = SutDefaultAuthorizationHandler.GetAuthorizationResult(Options, AuthorizationContextTestsFixture.AuthorizationContext);
            authorizationResult.Result.IsAuthorized.Should().Be(true);
        }

        [Test]
        public void GetAuthorizationResult_WhenTheUserInRoleIsTier2_ThenDontAllowTheUserToViewTeamPage()
        {
            //Arrange
            AuthorizationContextTestsFixture.SetData(AuthorizationConstants.TeamRoute);

            //Act
            AuthorizationContextTestsFixture.AuthorizationContext.ToString();

            //Assert
            var authorizationResult = SutDefaultAuthorizationHandler.GetAuthorizationResult(Options, AuthorizationContextTestsFixture.AuthorizationContext);
            authorizationResult.Result.IsAuthorized.Should().Be(false);
        }

        [Test]
        public void GetAuthorizationResult_WhenTheUserInRoleIsTier2_ThenDontAllowTheUserToViewHomePage()
        {
            //Arrange
            AuthorizationContextTestsFixture.SetData(AuthorizationConstants.TeamRoute);

            //Act
            AuthorizationContextTestsFixture.AuthorizationContext.ToString();

            //Assert
            var authorizationResult = SutDefaultAuthorizationHandler.GetAuthorizationResult(Options, AuthorizationContextTestsFixture.AuthorizationContext);
            authorizationResult.Result.IsAuthorized.Should().Be(false);
        }

    }


    public class AuthorizationContextTestsFixture
    {
        public IAuthorizationContext AuthorizationContext { get; set; }
        protected Mock<HttpContextBase> MockContextBase;
        protected Mock<HttpRequestBase> MockRequestBase;
        protected Mock<HttpResponseBase> MockResponseBase;
        protected Mock<IRouteHandler> MockRouteHandler { get; set; }
            


        public AuthorizationContextTestsFixture()
        {           
            AuthorizationContext = new AuthorizationContext();
            MockContextBase = new Mock<HttpContextBase>();
            MockRequestBase = new Mock<HttpRequestBase>();
            MockResponseBase = new Mock<HttpResponseBase>();
            MockRouteHandler = new Mock<IRouteHandler>();
            MockContextBase.Setup(x => x.Request).Returns(MockRequestBase.Object);
            MockContextBase.Setup(x => x.Response).Returns(MockResponseBase.Object);
        }

      
        public AuthorizationContextTestsFixture SetData(string url)
        {

            var resource = new Resource { Value = url };
            AuthorizationContext.Set("Resource", resource);            

            var claimsIdentity = new ClaimsIdentity(new[]
            {
                new Claim(DasClaimTypes.Id, "UserRef"),
                new Claim(DasClaimTypes.Email, "Email"),
                new Claim("sub", "UserRef"),
            });
            claimsIdentity.AddClaim(new Claim(claimsIdentity.RoleClaimType, AuthorizationConstants.Tier2User));
            var principal = new ClaimsPrincipal(claimsIdentity);
            MockContextBase.Setup(c => c.User).Returns(principal);
            AuthorizationContext.Set("ClaimsIdentity", claimsIdentity);
            
            return this;
        }      

        
    }

}