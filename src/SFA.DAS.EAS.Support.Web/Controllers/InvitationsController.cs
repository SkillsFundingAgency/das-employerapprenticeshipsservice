using System.Net;
using Newtonsoft.Json;
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
            ResponseUrl = $"/resource/invitemember/{id}",
        };

        return View(model);
    }

    public class CreateInvitationRequest
    {
        public string NameOfPersonBeingInvited { get; set; }

        public string EmailOfPersonBeingInvited { get; set; }

        public int RoleOfPersonBeingInvited { get; set; }
        public string SupportUserEmail { get; set; }
    }

    [HttpPost]
    [Route("{id}")]
    public async Task<IActionResult> SendInvitation(string id, [FromBody] CreateInvitationRequest request)
    {
        logger.LogWarning("InvitationsController.SendInvitation called.");
        
        var model = new SendInvitationCompletedModel
        {
            ReturnToTeamUrl = string.Format($"/resource?key={SupportServiceResourceKey.EmployerAccountTeam}&id={{0}}", id),
            MemberEmail = request.EmailOfPersonBeingInvited
        };

        try
        {
            await accountsApiService.SendInvitation(id, request.EmailOfPersonBeingInvited, request.NameOfPersonBeingInvited, request.SupportUserEmail, request.RoleOfPersonBeingInvited);
            
            model.Success = true;
        }
        catch (Exception exception)
        {
            logger.LogError(exception, $"{nameof(InvitationsController)}.{nameof(SendInvitation)} caught exception.");
        }
        
        logger.LogWarning("InvitationsController.SendInvitation model {Model}.", model);
        
        return View("Confirm", model);
    }

    [HttpGet]
    [Route("resend/{id}")]
    public async Task<IActionResult> Resend(string id, string email, string sid)
    {
        email = WebUtility.UrlDecode(email);

        var model = new SendInvitationCompletedModel
        {
            MemberEmail = email,
            ReturnToTeamUrl = string.Format($"/resource?key={SupportServiceResourceKey.EmployerAccountTeam}&id={{0}}", id)
        };

        try
        {
            await accountsApiService.ResendInvitation(id, email, sid);
            
            model.Success = true;
        }
        catch (Exception exception)
        {
            logger.LogError(exception, $"{nameof(InvitationsController)}.{nameof(Resend)} caught exception.");
        }

        return View("Confirm", model);
    }

    [HttpGet]
    [Route("confirm/{id}")]
    public IActionResult Confirm(string id, string email, bool success)
    {
        logger.LogWarning("InvitationsController.Confirm called.");
        
        var model = new SendInvitationCompletedModel
        {
            Success = success,
            MemberEmail = WebUtility.UrlDecode(email),
            ReturnToTeamUrl = string.Format($"/resource?key={SupportServiceResourceKey.EmployerAccountTeam}&id={{0}}", id)
        };
        
        logger.LogWarning("{Controller}.{Action}. Model: {Model}", nameof(InvitationsController), nameof(Confirm), JsonConvert.SerializeObject(model));

        return View(model);
    }
}