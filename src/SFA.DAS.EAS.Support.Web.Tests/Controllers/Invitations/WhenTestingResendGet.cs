using System.Security.Claims;
using FluentAssertions;
using FluentAssertions.Execution;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using SFA.DAS.EAS.Application.Infrastructure;
using SFA.DAS.EAS.Support.ApplicationServices.Models;
using SFA.DAS.EAS.Support.ApplicationServices.Services;
using SFA.DAS.EAS.Support.Web.Controllers;
using SFA.DAS.EAS.Support.Web.Models;
using SFA.DAS.Testing.AutoFixture;

namespace SFA.DAS.EAS.Support.Web.Tests.Controllers.Invitations;

public class WhenTestingResendGet
{
    [Test, MoqAutoData]
    public async Task ItShouldReturnViewAndModelOnSuccess(
        string hashedAccountId,
        string email,
        string externalUserId,
        Mock<IAccountHandler> accountHandler,
        string supportUserEmail
    )
    {
        var sut = new InvitationsController(accountHandler.Object, Mock.Of<ILogger<InvitationsController>>());
        sut.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext
            {
                User = new ClaimsPrincipal(new ClaimsIdentity(new List<Claim>
                {
                    new(EmployerClaims.IdamsUserIdClaimTypeIdentifier, externalUserId),
                })),
            }
        };

        var actual = await sut.Resend(hashedAccountId, email, supportUserEmail);

        using (new AssertionScope())
        {
            actual.Should().NotBeNull();
            actual.Should().BeOfType<ViewResult>();
            ((ViewResult)actual).ViewName.Should().Be("Confirm");

            var model = ((ViewResult)actual).Model as ResendInvitationCompletedModel;

            model?.Should().BeOfType<ResendInvitationCompletedModel>();
            model?.Success.Should().BeTrue();
            model?.MemberEmail.Should().Be(email);
            model?.ReturnToTeamUrl.Should().Be(string.Format($"/resource?key={SupportServiceResourceKey.EmployerAccountTeam}&id={{0}}", hashedAccountId));

            accountHandler.Verify(x => x.ResendInvitation(hashedAccountId, email, email, supportUserEmail), Times.Once);
        }
    }
}