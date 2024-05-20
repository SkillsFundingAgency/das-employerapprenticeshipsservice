using System.Security.Claims;
using FluentAssertions;
using FluentAssertions.Execution;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using SFA.DAS.EAS.Account.Api.Types;
using SFA.DAS.EAS.Application.Infrastructure;
using SFA.DAS.EAS.Domain.Models;
using SFA.DAS.EAS.Support.ApplicationServices.Models;
using SFA.DAS.EAS.Support.ApplicationServices.Services;
using SFA.DAS.EAS.Support.Web.Controllers;
using SFA.DAS.EAS.Support.Web.Models;
using SFA.DAS.Testing.AutoFixture;

namespace SFA.DAS.EAS.Support.Web.Tests.Controllers.UserRoles;

public class WhenTestingUpdatePost
{
    [Test, MoqAutoData]
    public async Task ItShouldReturnTheConfirmViewAndModel(
        Mock<IAccountHandler> accountHandler,
        ICollection<TeamMemberViewModel> teamMembers,
        string hashedAccountId,
        string userRef,
        Role oldRole,
        Role newRole,
        string externalUserId,
        string teamMemberName,
        string email,
        string supportUserEmail)
    {
        var sut = new UserRolesController(accountHandler.Object, Mock.Of<ILogger<UserRolesController>>());
        sut.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext
            {
                User = new ClaimsPrincipal(new ClaimsIdentity(new List<Claim>
                {
                    new(EmployerClaims.IdamsUserIdClaimTypeIdentifier, externalUserId),
                    new(ClaimTypes.Email, supportUserEmail),
                })),
            }
        };

        teamMembers.First().UserRef = userRef;
        teamMembers.First().Role = oldRole.ToString();
        teamMembers.First().Name = teamMemberName;
        teamMembers.First().Email = email;

        accountHandler.Setup(x => x.FindTeamMembers(hashedAccountId)).ReturnsAsync(new AccountReponse
        {
            StatusCode = SearchResponseCodes.Success,
            Account = new Core.Models.Account
            {
                HashedAccountId = hashedAccountId,
                TeamMembers = teamMembers
            }
        });

        var request = new UserRolesController.UpdateRoleRequest
        {
            Role = (int)newRole,
            SupportUserEmail = supportUserEmail
        };

        var actual = await sut.Update(hashedAccountId, userRef, request);

        using (new AssertionScope())
        {
            actual.Should().BeOfType<ViewResult>();
            ((ViewResult)actual).ViewName.Should().Be("Confirm");

            var model = ((ViewResult)actual).Model as ChangeRoleCompletedModel;

            model?.Should().BeOfType<ChangeRoleCompletedModel>();
            model?.Success.Should().BeTrue();
            model?.MemberEmail.Should().Be(email);
            model?.ReturnToTeamUrl.Should().Be(string.Format($"/resource?key={SupportServiceResourceKey.EmployerAccountTeam}&id={{0}}", hashedAccountId));
            model?.Role.Should().Be((int)newRole);

            accountHandler.Verify(x => x.ChangeRole(hashedAccountId, email, (int)newRole, supportUserEmail), Times.Once);
        }
    }

    [Test, MoqAutoData]
    public async Task ItShouldReturnSuccessFalseWhenExceptionIsCaught(
        Mock<IAccountHandler> accountHandler,
        string hashedAccountId,
        string userRef,
        Role oldRole,
        string supportUserEmail)
    {
        var sut = new UserRolesController(accountHandler.Object, Mock.Of<ILogger<UserRolesController>>());
        sut.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext()
        };

        accountHandler.Setup(x => x.FindTeamMembers(hashedAccountId)).ThrowsAsync(new ApplicationException("Test"));
        
        var request = new UserRolesController.UpdateRoleRequest
        {
            Role = (int)oldRole,
            SupportUserEmail = supportUserEmail
        };

        var actual = await sut.Update(hashedAccountId, userRef, request);

        var model = ((ViewResult)actual).Model as ChangeRoleCompletedModel;
        model?.Success.Should().BeFalse();
    }
}