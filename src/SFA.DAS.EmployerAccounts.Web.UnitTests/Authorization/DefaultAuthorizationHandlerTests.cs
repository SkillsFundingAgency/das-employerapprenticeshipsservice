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
using SFA.DAS.Authentication;
using SFA.DAS.EmployerAccounts.Configuration;
using SFA.DAS.EmployerAccounts.Interfaces;
using SFA.DAS.EmployerAccounts.Models.Account;

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
        private List<AuthorizationResource> resourceList { get; set; }
        private AuthorizationResource testAuthorizationResource;
        private Mock<EmployerAccountsConfiguration> _mockConfig;
        private Mock<IAuthenticationService> _mockAuthenticationService;

        [SetUp]
        public void Arrange()
        {
            _mockConfig = new Mock<EmployerAccountsConfiguration>();
            _mockAuthenticationService = new Mock<IAuthenticationService>();
            AuthorizationContextTestsFixture = new AuthorizationContextTestsFixture();
            MockIAuthorisationResourceRepository = new Mock<IAuthorisationResourceRepository>();
            Options = new List<string>();
            SutDefaultAuthorizationHandler = new DefaultAuthorizationHandler(MockIAuthorisationResourceRepository.Object,_mockConfig.Object,_mockAuthenticationService.Object);
            testAuthorizationResource = new AuthorizationResource
            {
                Name = "Test",
                Value = Guid.NewGuid().ToString()
            };
            resourceList = new List<AuthorizationResource>
            {
                testAuthorizationResource
            };

            MockIAuthorisationResourceRepository.Setup(x => x.Get(It.IsAny<ClaimsIdentity>())).Returns(resourceList);
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

        [Theory]
        [TestCase("Tier1User")]
        [TestCase("Tier2User")]
        public void GetAuthorizationResult_WhenTheUserIsConsoleUser_ThenAllowTheUserToViewTeamPage(string role)
        {
            //Arrange
             AuthorizationContextTestsFixture.SetData(testAuthorizationResource.Value,role);

            //Act
            AuthorizationContextTestsFixture.AuthorizationContext.ToString();

            //Assert
            var authorizationResult = SutDefaultAuthorizationHandler.GetAuthorizationResult(Options, AuthorizationContextTestsFixture.AuthorizationContext);
            authorizationResult.Result.IsAuthorized.Should().Be(true);
        }

        [Test]
        [Theory]
        [TestCase("Tier1User")]
        [TestCase("Tier2User")]
        public void GetAuthorizationResult_WhenTheUserInRoleIsTier2AndResourceNotSet_ThenAuthorizedTheUser(string role)
        {
            //Arrange
            AuthorizationContextTestsFixture.SetDataTier2UserNoResource(role);

            //Act
            AuthorizationContextTestsFixture.AuthorizationContext.ToString();

            //Assert
            var authorizationResult = SutDefaultAuthorizationHandler.GetAuthorizationResult(Options, AuthorizationContextTestsFixture.AuthorizationContext);
            authorizationResult.Result.IsAuthorized.Should().Be(false);
        }

        [Test]
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


        public AuthorizationContextTestsFixture SetData(string url, string role)
        {
            var resource = new Resource { Value = url };
            AuthorizationContext.Set("Resource", resource);

            var claimsIdentity = new ClaimsIdentity();
            claimsIdentity.AddClaim(new Claim(claimsIdentity.RoleClaimType, role));
            var principal = new ClaimsPrincipal(claimsIdentity);
            MockContextBase.Setup(c => c.User).Returns(principal);
            AuthorizationContext.Set("ClaimsIdentity", claimsIdentity);

            return this;
        }
        
        public AuthorizationContextTestsFixture SetDataTier2UserNoResource(string role)
        {
            var claimsIdentity = new ClaimsIdentity(new[]
            {
                new Claim(DasClaimTypes.Id, "UserRef"),
                new Claim(DasClaimTypes.Email, "Email"),
                new Claim("sub", "UserRef"),
            });
            claimsIdentity.AddClaim(new Claim(claimsIdentity.RoleClaimType, role));
            var principal = new ClaimsPrincipal(claimsIdentity);
            MockContextBase.Setup(c => c.User).Returns(principal);
            AuthorizationContext.Set("ClaimsIdentity", claimsIdentity);

            return this;
        }
        
        public AuthorizationContextTestsFixture SetDataNotTier2User()
        {
            var claimsIdentity = new ClaimsIdentity();
            var principal = new ClaimsPrincipal(claimsIdentity);
            MockContextBase.Setup(c => c.User).Returns(principal);
            AuthorizationContext.Set("ClaimsIdentity", claimsIdentity);

            return this;
        }
    }

}