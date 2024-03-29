﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Configuration;
using SFA.DAS.EAS.Web.Authentication;
using SFA.DAS.EAS.Web.Extensions;

namespace SFA.DAS.EAS.Web.Controllers;

[Route("invitations")]
public class InvitationController : Controller
{
    public IConfiguration Configuration { get; set; }
    public InvitationController(IConfiguration _configuration)
    {
        Configuration = _configuration;
    }
    [Route("invite")]
    public IActionResult Invite()
    {
        return Redirect(Url.EmployerAccountsAction("invitations/invite", Configuration, false));
    }

    [HttpGet("")]
    [Authorize(Policy = nameof(PolicyNames.HasUserAccount))]
    public IActionResult All()
    {
        return Redirect(Url.EmployerAccountsAction("invitations", Configuration, false));
    }

    [HttpGet]
    [Authorize(Policy = nameof(PolicyNames.HasUserAccount))]
    [Route("view")]
    public IActionResult Details(string invitationId)
    {
        return Redirect(Url.EmployerAccountsAction($"invitations/view?invitationId={invitationId}", Configuration, false));
    }

    [HttpGet]
    [Route("register-and-accept")]
    public IActionResult AcceptInvitationNewUser()
    {
        return Redirect(Url.EmployerAccountsAction("invitations/register-and-accept", Configuration, false));
    }

    [HttpGet]
    [Route("accept")]
    public IActionResult AcceptInvitationExistingUser()
    {
        return Redirect(Url.EmployerAccountsAction("invitations/accept", Configuration, false));
    }
}