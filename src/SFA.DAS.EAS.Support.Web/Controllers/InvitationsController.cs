﻿using System.Net;
using System.Web;
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
    }

    [HttpPost]
    [Route("{id}")]
    public async Task<IActionResult> SendInvitation(string id, [FromBody] CreateInvitationRequest request)
    {
        var model = new SendInvitationCompletedModel
        {
            ReturnToTeamUrl = string.Format($"/resource?key={SupportServiceResourceKey.EmployerAccountTeam}&id={{0}}", id),
            MemberEmail = request.EmailOfPersonBeingInvited
        };

        try
        {
            await accountsApiService.SendInvitation(id, request.EmailOfPersonBeingInvited, request.NameOfPersonBeingInvited, request.RoleOfPersonBeingInvited);

            model.Success = true;
        }
        catch (Exception exception)
        {
            logger.LogError(exception, $"{nameof(InvitationsController)}.{nameof(SendInvitation)} caught exception.");
        }

        return View("Confirm", model);
    }

    [HttpGet]
    [Route("resend/{id}")]
    public async Task<IActionResult> Resend(string id, string email)
    {
        var decodedEmail = Uri.UnescapeDataString(email);
        
        var resendInvitationSuccess = true;

        try
        {
            await accountsApiService.ResendInvitation(id, decodedEmail);
        }
        catch (Exception exception)
        {
            resendInvitationSuccess = false;
            logger.LogError(exception, $"{nameof(InvitationsController)}.{nameof(Resend)} caught exception.");
        }
        
        var model = new SendInvitationCompletedModel
        {
            Success = resendInvitationSuccess,
            MemberEmail = email,
            ReturnToTeamUrl = string.Format($"/resource?key={SupportServiceResourceKey.EmployerAccountTeam}&id={{0}}", id)
        };

        return View("Confirm", model);
    }
}