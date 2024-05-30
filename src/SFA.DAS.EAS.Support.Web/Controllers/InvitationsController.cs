using System.Net;
using SFA.DAS.EAS.Account.Api.Types;
using SFA.DAS.EAS.Application.Services.EmployerAccountsApi;
using SFA.DAS.EAS.Support.ApplicationServices.Models;
using SFA.DAS.EAS.Support.Web.Authorization;
using SFA.DAS.EAS.Support.Web.Models;

namespace SFA.DAS.EAS.Support.Web.Controllers;

[Route("invitations")]
[Authorize(Policy = PolicyNames.Default)]
public class InvitationsController(ILogger<InvitationsController> logger, IEmployerAccountsApiService accountsApiService) : Controller
{
    [HttpGet]
    [Route("{id}")]
    public IActionResult Index(string id)
    {
        var model = new InvitationViewModel
        {
            HashedAccountId = id,
            ResponseUrl = $"/resource/invitemember/{id}"
        };

        return View(model);
    }


    [HttpPost]
    [Route("{id}")]
    public async Task<IActionResult> SendInvitation(string id, string email, string fullName, string sid, int role)
    {
        var model = new SendInvitationCompletedModel
        {
            ReturnToTeamUrl = string.Format($"/resource?key={SupportServiceResourceKey.EmployerAccountTeam}&id={{0}}", id),
            Success = true,
            MemberEmail = email
        };

        try
        {
            await accountsApiService.SendInvitation(id, email, fullName, sid, role);
        }
        catch (Exception exception)
        {
            logger.LogError(exception, $"{nameof(InvitationsController)}.{nameof(SendInvitation)} caught exception.");
            model.Success = false;
        }

        return View("Confirm", model);
    }

    [HttpGet]
    [Route("resend/{id}")]
    public async Task<IActionResult> Resend(string id, string email, string sid)
    {
        email = WebUtility.UrlDecode(email);

        var model = new SendInvitationCompletedModel
        {
            Success = true,
            MemberEmail = email,
            ReturnToTeamUrl = string.Format($"/resource?key={SupportServiceResourceKey.EmployerAccountTeam}&id={{0}}", id)
        };

        try
        {
            await accountsApiService.ResendInvitation(
                id,
                email,
                sid
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