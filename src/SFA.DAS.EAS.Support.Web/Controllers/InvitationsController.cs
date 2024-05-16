using System.Net;
using System.Security.Claims;
using ASP;
using Newtonsoft.Json;
using SFA.DAS.EAS.Support.ApplicationServices.Models;
using SFA.DAS.EAS.Support.ApplicationServices.Services;
using SFA.DAS.EAS.Support.Web.Authorization;
using SFA.DAS.EAS.Support.Web.Models;

namespace SFA.DAS.EAS.Support.Web.Controllers;

[Route("invitations")]
[Authorize(Policy = PolicyNames.Default)]
public class InvitationsController(IAccountHandler accountHandler, ILogger<InvitationsController> logger) : Controller
{
    [HttpGet]
    [Route("resend/{id}")]
    public async Task<IActionResult> Resend(string id, string email)
    {
        email = WebUtility.HtmlDecode(email);
        
        var model = new ResendInvitationCompletedModel
        {
            Success = true,
            MemberEmail = email,
            ReturnToTeamUrl = string.Format($"/resource?key={SupportServiceResourceKey.EmployerAccountTeam}&id={{0}}", id)
        };

        logger.LogWarning("InvitationsController.Resend user claims: {Claims}",  JsonConvert.SerializeObject(HttpContext.User.Claims.Select(x => new { x.Type, x.Value})));

        var supportUserEmail = HttpContext.User.FindFirstValue(ClaimTypes.Email);

        try
        {
            await accountHandler.ResendInvitation(
                id,
                email,
                email,
                supportUserEmail
            );
        }
        catch (Exception exception)
        {
            logger.LogError(exception, $"{nameof(InvitationsController)}.{nameof(Resend)} caught exception.");
            model.Success = false;
        }

        return View("Confirm", model);
    }
}