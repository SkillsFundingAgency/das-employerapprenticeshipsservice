using Moq;
using NUnit.Framework;
using SFA.DAS.Authorization.Context;
using System.Collections.Generic;
using FluentAssertions;
using System.Threading.Tasks;
using System.Web.Routing;
using System.Web;
using System.Security.Claims;
using SFA.DAS.EmployerAccounts.Web.Authorization;
using AuthorizationContext = SFA.DAS.Authorization.Context.AuthorizationContext;
using SFA.DAS.EmployerUsers.WebClientComponents;
using static SFA.DAS.EmployerAccounts.Web.Authorization.ImpersonationAuthorizationContext;
using System;
using SFA.DAS.EmployerAccounts.Interfaces;
using SFA.DAS.EmployerAccounts.Models.Account;
using SFA.DAS.EmployerAccounts.Services;
using SFA.DAS.EmployerAccounts.Configuration;

namespace SFA.DAS.EmployerAccounts.Web.UnitTests.Authorization
{
    [TestFixture]
    public class DefaultAuthorizationHandlerTests
    {
        public List<string> Options { get; set; }
        public IAuthorizationContext AuthorizationContext { get; set; }
        public DefaultAuthorizationHandler SutDefaultAuthorizationHandler { get; set; }
        public AuthorizationContextTestsFixture AuthorizationContextTestsFixture { get; set; }
        private Mock<IAuthorisationResourceRepository> MockIAuthorisationResourceRepository { get; set; }
        private List<ResourceRoute> resourceList { get; set; }        

        [SetUp]
        public void Arrange()
        {
            AuthorizationContextTestsFixture = new AuthorizationContextTestsFixture();
            MockIAuthorisationResourceRepository = new Mock<IAuthorisationResourceRepository>();
            Options = new List<string>();
            SutDefaultAuthorizationHandler = new DefaultAuthorizationHandler(MockIAuthorisationResourceRepository.Object);
            resourceList = new AuthorisationResourceRepository().Get();
            MockIAuthorisationResourceRepository.Setup(x => x.Get()).Returns(resourceList);
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

        
        [TestCase(AuthorizedTier2Route.TeamViewRoute, true)]        
        [TestCase(AuthorizedTier2Route.TeamInvite, true)]
        [TestCase(AuthorizedTier2Route.TeamInviteComplete, true)]
        [TestCase(AuthorizedTier2Route.TeamMemberRemove, true)]
        [TestCase(AuthorizedTier2Route.TeamReview, true)]
        [TestCase(AuthorizedTier2Route.TeamMemberRoleChange, true)]
        [TestCase(AuthorizedTier2Route.TeamMemberInviteResend, true)]
        [TestCase(AuthorizedTier2Route.TeamMemberInviteCancel, true)]
        [TestCase(AuthorizedTier2Route.TeamRoute, false)]
        public void GetAuthorizationResult_WhenTheUserInRoleIsTier2_ThenAllowTheUserToViewTeamPage(string url, bool expected)
        {
            //Arrange
             AuthorizationContextTestsFixture.SetData(url);

            //Act
            AuthorizationContextTestsFixture.AuthorizationContext.ToString();

            //Assert
            var authorizationResult = SutDefaultAuthorizationHandler.GetAuthorizationResult(Options, AuthorizationContextTestsFixture.AuthorizationContext);
            authorizationResult.Result.IsAuthorized.Should().Be(expected);
        }

        [Test]
        //Tests missing for if Tier2User user and no Reseource set in the context.
        public void GetAuthorizationResult_WhenTheUserInRoleIsTier2AndResourceNotSet_ThenAuthorizedTheUser()
        {
            //Arrange
            AuthorizationContextTestsFixture.SetDataTier2UserNoResource();

            //Act
            AuthorizationContextTestsFixture.AuthorizationContext.ToString();

            //Assert
            var authorizationResult = SutDefaultAuthorizationHandler.GetAuthorizationResult(Options, AuthorizationContextTestsFixture.AuthorizationContext);
            authorizationResult.Result.IsAuthorized.Should().Be(false);
        }

        [Test]
        //Tests missing for when ClaimsIdentity in context but not a Tier2User user.
        public void GetAuthorizationResult_WhenTheUserInRoleINotTier2AndClaimsSet_ThenAuthorizedTheUser()
        {
            //Arrange
            AuthorizationContextTestsFixture.SetDataNotTier2User();

            //Act
            AuthorizationContextTestsFixture.AuthorizationContext.ToString();

            //Assert
            var authorizationResult = SutDefaultAuthorizationHandler.GetAuthorizationResult(Options, AuthorizationContextTestsFixture.AuthorizationContext);
            authorizationResult.Result.IsAuthorized.Should().Be(true);
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

        ////Tests missing for if Tier2User user and no Reseource set in the context.
        public AuthorizationContextTestsFixture SetDataTier2UserNoResource()
        {
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

        //Tests missing for when ClaimsIdentity in context but not a Tier2User user.
        public AuthorizationContextTestsFixture SetDataNotTier2User()
        {
            var claimsIdentity = new ClaimsIdentity(new[]
            {
                new Claim(DasClaimTypes.Id, "UserRef"),
                new Claim(DasClaimTypes.Email, "Email"),
                new Claim("sub", "UserRef"),
            });
            
            var principal = new ClaimsPrincipal(claimsIdentity);
            MockContextBase.Setup(c => c.User).Returns(principal);
            AuthorizationContext.Set("ClaimsIdentity", claimsIdentity);

            return this;
        }
    }

}