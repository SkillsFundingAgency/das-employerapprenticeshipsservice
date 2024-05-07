using System.Security.Claims;
using SFA.DAS.EAS.Application.Infrastructure;
using SFA.DAS.EAS.Support.ApplicationServices.Services;
using SFA.DAS.EAS.Support.Web.Models;

namespace SFA.DAS.EAS.Support.Web.Controllers;

public class InvitationsController(IAccountHandler accountHandler, ILogger<InvitationsController> logger) : Controller
{
    [HttpPost]
    [Route("{id}/{userRef}")]
    public async Task<IActionResult> Resend(string id, string userRef, int role)
    {
        var model = new ResendInvitationCompletedModel { Success = true};

        try
        {
            var accountResponse = await accountHandler.FindTeamMembers(id);

            var teamMember = accountResponse.Account.TeamMembers.Single(x => x.UserRef == userRef);

            // await accountHandler.ResendInvitation(
            //     id,
            //     teamMember.Email,
            //     teamMember.Name.Split(' ').FirstOrDefault(),
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