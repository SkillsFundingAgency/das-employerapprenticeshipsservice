using System.Security.Claims;
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

[TestFixture]
public class WhenTestingResendGet
{
    [Test, MoqAutoData]
    public async Task ItShouldReturnViewAndModelOnSuccess(
        string hashedAccountId,
        string email,
        string externalUserId,
        Mock<IAccountHandler> accountHandler
        )
    {
        var sut = new InvitationsController(accountHandler.Object, Mock.Of<ILogger<InvitationsController>>());
        sut.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext
            {
                User = new ClaimsPrincipal(new ClaimsIdentity(new List<Claim> { new(EmployerClaims.IdamsUserIdClaimTypeIdentifier, externalUserId) })),
            }
        };
        
        var actual = await sut!.Resend(hashedAccountId, email);

        Assert.Multiple(() =>
        {
            Assert.That(actual, Is.Not.Null);
            Assert.That(actual, Is.InstanceOf<ViewResult>());
            Assert.That(((ViewResult)actual).ViewName,  Is.EqualTo("Confirm"));
            
            var model = ((ViewResult)actual).Model as ResendInvitationCompletedModel;
            
            Assert.That(model, Is.InstanceOf<ResendInvitationCompletedModel>());
            Assert.That(model.Success, Is.True);
            Assert.That(model.MemberEmail, Is.EqualTo(email));
            Assert.That(model.ReturnToTeamUrl, Is.EqualTo(string.Format($"/resource?key={SupportServiceResourceKey.EmployerAccountTeam}&id={{0}}", hashedAccountId)));
            
            accountHandler.Verify(x => x.ResendInvitation(hashedAccountId, email, email, externalUserId), Times.Once);
        });
    }
}