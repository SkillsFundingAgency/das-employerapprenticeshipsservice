using System.Security.Claims;
using AutoFixture.NUnit3;
using FluentAssertions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.Extensions.Options;
using Microsoft.Identity.Client;
using Newtonsoft.Json;
using SFA.DAS.EmployerAccounts.Infrastructure;
using SFA.DAS.EmployerAccounts.Infrastructure.OuterApi.Responses.UserAccounts;
using SFA.DAS.EmployerAccounts.Models.UserAccounts;
using SFA.DAS.EmployerAccounts.Services;
using SFA.DAS.EmployerAccounts.Web.Authorization;
using SFA.DAS.EmployerAccounts.Web.RouteValues;
using SFA.DAS.Testing.AutoFixture;

namespace SFA.DAS.EmployerAccounts.Web.UnitTests.AppStart;

public class WhenHandlingUserOutsideAccountAuthorization
{
    [Test, MoqAutoData]
    public async Task Then_Returns_False_If_Navigating_Within_Employer_Account_Context(
        string userId,
        string accountId,
        EmployerAccountOwnerRequirement ownerRequirement,
        [Frozen] Mock<IHttpContextAccessor> httpContextAccessor,
        [Frozen] Mock<IOptions<EmployerAccountsConfiguration>> configuration,
        EmployerAccountAuthorisationHandler authorizationHandler)
    {
        //Arrange
        configuration.Object.Value.UseGovSignIn = true;

        var userClaim = new Claim(ClaimTypes.NameIdentifier, userId);
        var claimsPrinciple = new ClaimsPrincipal(new[] { new ClaimsIdentity(new[] { userClaim }) });
        var context = new AuthorizationHandlerContext(new[] { ownerRequirement }, claimsPrinciple, null);
        var httpContext = new DefaultHttpContext(new FeatureCollection());
        httpContext.Request.RouteValues.Add(RouteValueKeys.HashedAccountId, accountId.ToUpper());
        httpContextAccessor.Setup(x => x.HttpContext).Returns(httpContext);

        //Act
        var actual = await authorizationHandler.IsOutsideAccount(context);

        //Assert
        actual.Should().BeFalse();
    }

    [Test, MoqAutoData]
    public async Task Then_Returns_True_If_Navigating_Outside_Account_And_User_Is_Authorized_For_Gov_SignIn(
        string userId,
        EmployerAccountOwnerRequirement ownerRequirement,
        [Frozen] Mock<IHttpContextAccessor> httpContextAccessor,
        [Frozen] Mock<IOptions<EmployerAccountsConfiguration>> configuration,
        EmployerAccountAuthorisationHandler authorizationHandler)
    {
        //Arrange
        configuration.Object.Value.UseGovSignIn = true;

        var userClaim = new Claim(ClaimTypes.NameIdentifier, userId);
        var claimsPrinciple = new ClaimsPrincipal(new[] { new ClaimsIdentity(new[] { userClaim }) });
        var context = new AuthorizationHandlerContext(new[] { ownerRequirement }, claimsPrinciple, null);
        var httpContext = new DefaultHttpContext(new FeatureCollection());
        httpContextAccessor.Setup(x => x.HttpContext).Returns(httpContext);

        //Act
        var actual = await authorizationHandler.IsOutsideAccount(context);

        //Assert
        actual.Should().BeTrue();
    }

    [Test, MoqAutoData]
    public async Task Then_Returns_True_If_Navigating_Outside_Account_And_User_Is_Authorized_NOT_Gov_SignIn(
        string userId,
        EmployerAccountOwnerRequirement ownerRequirement,
        [Frozen] Mock<IHttpContextAccessor> httpContextAccessor,
        [Frozen] Mock<IOptions<EmployerAccountsConfiguration>> configuration,
        EmployerAccountAuthorisationHandler authorizationHandler)
    {
        //Arrange
        configuration.Object.Value.UseGovSignIn = false;

        var userClaim = new Claim(EmployerClaims.IdamsUserIdClaimTypeIdentifier, userId);
        var claimsPrinciple = new ClaimsPrincipal(new[] { new ClaimsIdentity(new[] { userClaim }) });
        var context = new AuthorizationHandlerContext(new[] { ownerRequirement }, claimsPrinciple, null);
        var httpContext = new DefaultHttpContext(new FeatureCollection());
        httpContextAccessor.Setup(x => x.HttpContext).Returns(httpContext);

        //Act
        var actual = await authorizationHandler.IsOutsideAccount(context);

        //Assert
        actual.Should().BeTrue();
    }
}