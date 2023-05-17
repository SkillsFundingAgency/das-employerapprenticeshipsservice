using System.Security.Claims;
using AutoFixture.NUnit3;
using FluentAssertions;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using SFA.DAS.EmployerAccounts.Infrastructure;
using SFA.DAS.EmployerAccounts.Models.UserAccounts;
using SFA.DAS.EmployerAccounts.Services;
using SFA.DAS.EmployerAccounts.Web.Handlers;
using SFA.DAS.Testing.AutoFixture;

namespace SFA.DAS.EmployerAccounts.Web.UnitTests.Handlers;

public class WhenPopulatingAccountClaims
{
    [Test, MoqAutoData]
    public async Task Then_The_Claims_Are_Populated_For_Gov_User(
        string nameIdentifier,
        string idamsIdentifier,
        string emailAddress,
        EmployerUserAccounts accountData,
        [Frozen] Mock<IUserAccountService> accountService,
        [Frozen] Mock<IOptions<EmployerAccountsConfiguration>> accountsConfiguration,
        EmployerAccountPostAuthenticationClaimsHandler handler)
    {
        accountData.IsSuspended = true;
        accountsConfiguration.Object.Value.UseGovSignIn = true;
        var tokenValidatedContext = ArrangeTokenValidatedContext(nameIdentifier, idamsIdentifier, emailAddress);
        accountService.Setup(x => x.GetUserAccounts(nameIdentifier, emailAddress)).ReturnsAsync(accountData);

        var actual = await handler.GetClaims(tokenValidatedContext);

        accountService.Verify(x => x.GetUserAccounts(nameIdentifier, emailAddress), Times.Once);
        accountService.Verify(x => x.GetUserAccounts(idamsIdentifier, emailAddress), Times.Never);
        actual.Should().ContainSingle(c => c.Type.Equals(EmployerClaims.AccountsClaimsTypeIdentifier));
        var actualClaimValue = actual.First(c => c.Type.Equals(EmployerClaims.AccountsClaimsTypeIdentifier)).Value;
        JsonConvert.SerializeObject(accountData.EmployerAccounts.ToDictionary(k => k.AccountId)).Should().Be(actualClaimValue);
        actual.First(c => c.Type.Equals(EmployerClaims.IdamsUserIdClaimTypeIdentifier)).Value.Should().Be(accountData.EmployerUserId);
        actual.First(c => c.Type.Equals(EmployerClaims.IdamsUserDisplayNameClaimTypeIdentifier)).Value.Should().Be(accountData.FirstName + " " + accountData.LastName);
        actual.First(c => c.Type.Equals(EmployerClaims.IdamsUserEmailClaimTypeIdentifier)).Value.Should().Be(emailAddress);
        actual.First(c => c.Type.Equals(DasClaimTypes.GivenName)).Value.Should().Be(accountData.FirstName);
        actual.First(c => c.Type.Equals(DasClaimTypes.FamilyName)).Value.Should().Be(accountData.LastName);
        actual.First(c => c.Type.Equals(ClaimTypes.AuthorizationDecision)).Value.Should().Be("Suspended");
    }
    
    [Test, MoqAutoData]
    public async Task Then_The_Claims_Are_Populated_For_Gov_User_With_No_Accounts(
        string nameIdentifier,
        string idamsIdentifier,
        string emailAddress,
        EmployerUserAccounts accountData,
        [Frozen] Mock<IUserAccountService> accountService,
        [Frozen] Mock<IOptions<EmployerAccountsConfiguration>> accountsConfiguration,
        EmployerAccountPostAuthenticationClaimsHandler handler)
    {
        accountData.IsSuspended = true;
        accountData.EmployerAccounts = new List<EmployerUserAccountItem>();
        accountData.FirstName = null;
        accountData.LastName = null;
        accountsConfiguration.Object.Value.UseGovSignIn = true;
        var tokenValidatedContext = ArrangeTokenValidatedContext(nameIdentifier, idamsIdentifier, emailAddress);
        accountService.Setup(x => x.GetUserAccounts(nameIdentifier, emailAddress)).ReturnsAsync(accountData);

        var actual = await handler.GetClaims(tokenValidatedContext);

        accountService.Verify(x => x.GetUserAccounts(nameIdentifier, emailAddress), Times.Once);
        accountService.Verify(x => x.GetUserAccounts(idamsIdentifier, emailAddress), Times.Never);
        actual.Should().Contain(c => c.Type.Equals(EmployerClaims.AccountsClaimsTypeIdentifier));
        actual.FirstOrDefault(c => c.Type.Equals(EmployerClaims.AccountsClaimsTypeIdentifier))?.Value.Should().Be("{}");
        
        actual.First(c => c.Type.Equals(EmployerClaims.IdamsUserIdClaimTypeIdentifier)).Value.Should().Be(accountData.EmployerUserId);
        actual.FirstOrDefault(c => c.Type.Equals(EmployerClaims.IdamsUserDisplayNameClaimTypeIdentifier))?.Value.Should().BeNull();
        actual.First(c => c.Type.Equals(EmployerClaims.IdamsUserEmailClaimTypeIdentifier)).Value.Should().Be(emailAddress);
        actual.FirstOrDefault(c => c.Type.Equals(DasClaimTypes.GivenName))?.Value.Should().BeNull();
        actual.FirstOrDefault(c => c.Type.Equals(DasClaimTypes.FamilyName))?.Value.Should().BeNull();
        actual.First(c => c.Type.Equals(ClaimTypes.AuthorizationDecision)).Value.Should().Be("Suspended");
    }
    
    [Test, MoqAutoData]
    public async Task Then_The_Claims_Are_Populated_For_Gov_User_And_Not_Suspended_Set(
        string nameIdentifier,
        string idamsIdentifier,
        string emailAddress,
        EmployerUserAccounts accountData,
        [Frozen] Mock<IUserAccountService> accountService,
        [Frozen] Mock<IOptions<EmployerAccountsConfiguration>> accountsConfiguration,
        EmployerAccountPostAuthenticationClaimsHandler handler)
    {
        accountData.IsSuspended = false;
        accountsConfiguration.Object.Value.UseGovSignIn = true;
        var tokenValidatedContext = ArrangeTokenValidatedContext(nameIdentifier, idamsIdentifier, emailAddress);
        accountService.Setup(x => x.GetUserAccounts(nameIdentifier, emailAddress)).ReturnsAsync(accountData);

        var actual = await handler.GetClaims(tokenValidatedContext);

        accountService.Verify(x => x.GetUserAccounts(nameIdentifier, emailAddress), Times.Once);
        accountService.Verify(x => x.GetUserAccounts(idamsIdentifier, emailAddress), Times.Never);
        actual.Should().ContainSingle(c => c.Type.Equals(EmployerClaims.AccountsClaimsTypeIdentifier));
        var actualClaimValue = actual.First(c => c.Type.Equals(EmployerClaims.AccountsClaimsTypeIdentifier)).Value;
        JsonConvert.SerializeObject(accountData.EmployerAccounts.ToDictionary(k => k.AccountId)).Should().Be(actualClaimValue);
        actual.First(c => c.Type.Equals(EmployerClaims.IdamsUserIdClaimTypeIdentifier)).Value.Should().Be(accountData.EmployerUserId);
        actual.First(c => c.Type.Equals(EmployerClaims.IdamsUserDisplayNameClaimTypeIdentifier)).Value.Should().Be(accountData.FirstName + " " + accountData.LastName);
        actual.First(c => c.Type.Equals(EmployerClaims.IdamsUserEmailClaimTypeIdentifier)).Value.Should().Be(emailAddress);
        actual.FirstOrDefault(c => c.Type.Equals(ClaimTypes.AuthorizationDecision))?.Value.Should().BeNull();
    }

    [Test, MoqAutoData]
    public async Task Then_The_Claims_Are_Populated_For_EmployerUsers_User(
        string nameIdentifier,
        string idamsIdentifier,
        EmployerUserAccounts accountData,
        [Frozen] Mock<IUserAccountService> accountService,
        [Frozen] Mock<IOptions<EmployerAccountsConfiguration>> accountsConfiguration,
        EmployerAccountPostAuthenticationClaimsHandler handler)
    {
        var tokenValidatedContext = ArrangeTokenValidatedContext(nameIdentifier, idamsIdentifier, string.Empty);
        accountService.Setup(x => x.GetUserAccounts(idamsIdentifier, "")).ReturnsAsync(accountData);
        accountsConfiguration.Object.Value.UseGovSignIn = false;

        var actual = await handler.GetClaims(tokenValidatedContext);

        accountService.Verify(x => x.GetUserAccounts(nameIdentifier, string.Empty), Times.Never);
        accountService.Verify(x => x.GetUserAccounts(idamsIdentifier, string.Empty), Times.Once);
        actual.Should().ContainSingle(c => c.Type.Equals(EmployerClaims.AccountsClaimsTypeIdentifier));
        var actualClaimValue = actual.First(c => c.Type.Equals(EmployerClaims.AccountsClaimsTypeIdentifier)).Value;
        JsonConvert.SerializeObject(accountData.EmployerAccounts.ToDictionary(k => k.AccountId)).Should().Be(actualClaimValue);
        actual.FirstOrDefault(c => c.Type.Equals(EmployerClaims.IdamsUserIdClaimTypeIdentifier)).Should().NotBeNull();
        actual.FirstOrDefault(c => c.Type.Equals(EmployerClaims.IdamsUserDisplayNameClaimTypeIdentifier)).Should().BeNull();
    }

    private static TokenValidatedContext ArrangeTokenValidatedContext(string nameIdentifier, string idamsIdentifier, string emailAddress)
    {
        var identity = new ClaimsIdentity(new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, nameIdentifier),
            new(EmployerClaims.IdamsUserIdClaimTypeIdentifier, idamsIdentifier),
            new(ClaimTypes.Email, emailAddress),
            new(EmployerClaims.IdamsUserEmailClaimTypeIdentifier, emailAddress)
        });

        var claimsPrincipal = new ClaimsPrincipal(new ClaimsIdentity(identity));
        return new TokenValidatedContext(new DefaultHttpContext(), new AuthenticationScheme(",", "", typeof(TestAuthHandler)),
            new OpenIdConnectOptions(), Mock.Of<ClaimsPrincipal>(), new AuthenticationProperties())
        {
            Principal = claimsPrincipal
        };
    }


    private class TestAuthHandler : IAuthenticationHandler
    {
        public Task InitializeAsync(AuthenticationScheme scheme, HttpContext context)
        {
            throw new NotImplementedException();
        }

        public Task<AuthenticateResult> AuthenticateAsync()
        {
            throw new NotImplementedException();
        }

        public Task ChallengeAsync(AuthenticationProperties properties)
        {
            throw new NotImplementedException();
        }

        public Task ForbidAsync(AuthenticationProperties properties)
        {
            throw new NotImplementedException();
        }
    }
}