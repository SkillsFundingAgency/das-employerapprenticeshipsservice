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
    
    public class CreateInvitationRequest
    {
        public string HashedAccountId { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public int Role { get; set; }
        public string SupportUserEmail { get; set; }
    }
    
    [HttpPost]
    [Route("")]
    public async Task<IActionResult> SendInvitation([FromBody] CreateInvitationRequest request)
    {
        var model = new SendInvitationCompletedModel
        {
            ReturnToTeamUrl = string.Format($"/resource?key={SupportServiceResourceKey.EmployerAccountTeam}&id={{0}}", request.HashedAccountId),
            Success = true,
            MemberEmail = request.Email
        };

        try
        {
            await accountsApiService.SendInvitation(request.HashedAccountId, request.Email, request.Name, request.SupportUserEmail, request.Role);
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