﻿using Moq;
using NUnit.Framework;
using SFA.DAS.Authorization.Context;
using System.Collections.Generic;
using FluentAssertions;
using System.Threading.Tasks;
using System.Security.Claims;
using SFA.DAS.EmployerAccounts.Web.Authorization;
using AuthorizationContext = SFA.DAS.Authorization.Context.AuthorizationContext;
using SFA.DAS.EmployerUsers.WebClientComponents;
using static SFA.DAS.EmployerAccounts.Web.Authorization.ImpersonationAuthorizationContext;
using System;
using SFA.DAS.Authentication;
using SFA.DAS.EmployerAccounts.Configuration;
using SFA.DAS.EmployerAccounts.Extensions;
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
        private List<AuthorizationResource> ResourceList { get; set; }
        private AuthorizationResource _testAuthorizationResource;
        private EmployerAccountsConfiguration _configuration;
        private Mock<IAuthenticationService> _mockAuthenticationService;
        private readonly string SupportConsoleUsers = "Tier1User,Tier2User";
        private IUserContext _userContext;

        [SetUp]
        public void Arrange()
        {
            _configuration = new EmployerAccountsConfiguration
            {
                SupportConsoleUsers = SupportConsoleUsers
            };
            _mockAuthenticationService = new Mock<IAuthenticationService>();
            AuthorizationContextTestsFixture = new AuthorizationContextTestsFixture();
            MockIAuthorisationResourceRepository = new Mock<IAuthorisationResourceRepository>();
            Options = new List<string>();
            _userContext = new UserContext(_mockAuthenticationService.Object,_configuration);
            SutDefaultAuthorizationHandler = new DefaultAuthorizationHandler(MockIAuthorisationResourceRepository.Object,_userContext);
            _testAuthorizationResource = new AuthorizationResource
            {
                Name = "Test",
                Value = Guid.NewGuid().ToString()
            };
            ResourceList = new List<AuthorizationResource>
            {
                _testAuthorizationResource
            };

            MockIAuthorisationResourceRepository.Setup(x => x.Get(It.IsAny<ClaimsIdentity>())).Returns(ResourceList);
            AuthorizationContext = new AuthorizationContext();
        }
        

        [Test]
        public async Task GetAuthorizationResult_WhenTheUserInRoleIsNotSupportConsole_ThenAuthorizedTheUser()
        {
            //Act                        
            var authorizationResult = await SutDefaultAuthorizationHandler.GetAuthorizationResult(Options, AuthorizationContext);

            //Assert
            authorizationResult.IsAuthorized.Should().Be(true);
        }

        [Theory]
        [TestCase("Tier1User")]
        [TestCase("Tier2User")]
        [TestCase("TierUser")]
        public void GetAuthorizationResult_WhenTheUserIsConsoleUser_ThenAllowTheUserToViewTeamPage(string role)
        {
            //Arrange
             AuthorizationContextTestsFixture.SetData(_testAuthorizationResource.Value,role);

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
        public void GetAuthorizationResult_WhenTheUserInRoleIsSupportConsoleAndResourceNotSet_ThenAuthorizedTheUser(string role)
        {
            //Arrange
            AuthorizationContextTestsFixture.SetDataSupportConsoleUserNoResource(role);

            _mockAuthenticationService.Setup(m => m.HasClaim(ClaimsIdentity.DefaultRoleClaimType, role)).Returns(true);
            
            //Act
            AuthorizationContextTestsFixture.AuthorizationContext.ToString();

            //Assert
            var authorizationResult = SutDefaultAuthorizationHandler.GetAuthorizationResult(Options, AuthorizationContextTestsFixture.AuthorizationContext);
            authorizationResult.Result.IsAuthorized.Should().Be(false);
        }

        [Test]
        public void GetAuthorizationResult_WhenTheUserInRoleINotSupportConsoleAndClaimsSet_ThenAuthorizedTheUser()
        {
            //Arrange
            AuthorizationContextTestsFixture.SetDataNotSupportConsoleUser();

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
        
        public AuthorizationContextTestsFixture SetDataSupportConsoleUserNoResource(string role)
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
        
        public AuthorizationContextTestsFixture SetDataNotSupportConsoleUser()
        {
            var claimsIdentity = new ClaimsIdentity();
            var principal = new ClaimsPrincipal(claimsIdentity);
            MockContextBase.Setup(c => c.User).Returns(principal);
            AuthorizationContext.Set("ClaimsIdentity", claimsIdentity);
            return this;
        }
    }

}