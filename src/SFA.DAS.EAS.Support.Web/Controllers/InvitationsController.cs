using System.Security.Claims;
using SFA.DAS.EAS.Application.Infrastructure;
using SFA.DAS.EAS.Support.ApplicationServices.Models;
using SFA.DAS.EAS.Support.ApplicationServices.Services;
using SFA.DAS.EAS.Support.Web.Models;

namespace SFA.DAS.EAS.Support.Web.Controllers;

public class InvitationsController(IAccountHandler accountHandler, ILogger<InvitationsController> logger) : Controller
{
    [HttpPost]
    [Route("{id}/{email}")]
    public async Task<IActionResult> Resend(string id, string email)
    {
        var model = new ResendInvitationCompletedModel { 
            Success = true,
            MemberEmail = email,
            ReturnToTeamUrl = string.Format($"/resource?key={SupportServiceResourceKey.EmployerAccountTeam}&id={{0}}", id)
        };

        try
        {
            // await accountHandler.ResendInvitation(
            //     id,
            //     email,
            //     email,
            //     HttpContext.User.FindFirstValue(EmployerClaims.IdamsUserIdClaimTypeIdentifier)
            // );
        }
        catch (Exception exception)
        {
            logger.LogError(exception, $"{nameof(InvitationsController)}.{nameof(Resend)} caught exception.");
            model.Success = false;
        }
        
        return View("Confirm", model);
    }
}