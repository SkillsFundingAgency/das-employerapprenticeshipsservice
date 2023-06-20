using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using SFA.DAS.EmployerAccounts.Infrastructure;
using SFA.DAS.EmployerAccounts.Web.RouteValues;

namespace SFA.DAS.EmployerAccounts.Web.Controllers;

[Route("invitations")]
public class InvitationController : BaseController
{
    private readonly InvitationOrchestrator _invitationOrchestrator;

    private readonly EmployerAccountsConfiguration _configuration;
    
    public InvitationController(InvitationOrchestrator invitationOrchestrator,
        EmployerAccountsConfiguration configuration,
        ICookieStorageService<FlashMessageViewModel> flashMessage)
        : base(flashMessage)
    {
        _invitationOrchestrator = invitationOrchestrator ?? throw new ArgumentNullException(nameof(invitationOrchestrator));
        _configuration = configuration;
    }

    [Route("invite")]
    public IActionResult Invite()
    {
        if (string.IsNullOrEmpty(HttpContext.User.FindFirstValue(ControllerConstants.UserRefClaimKeyName)))
        {
            return View();
        }

        return RedirectToAction(ControllerConstants.IndexActionName, ControllerConstants.HomeControllerName);
    }

    [HttpGet]
    [Authorize]
    public async Task<IActionResult> All()
    {
        if (string.IsNullOrEmpty(HttpContext.User.FindFirstValue(ControllerConstants.UserRefClaimKeyName)))
        {
            return RedirectToAction(ControllerConstants.IndexActionName, ControllerConstants.HomeControllerName);
        }

        var model = await _invitationOrchestrator.GetAllInvitationsForUser(HttpContext.User.FindFirstValue(EmployerClaims.IdamsUserIdClaimTypeIdentifier));

        return View(model);
    }

    [HttpGet]
    [Authorize]
    [Route("view")]
    public async Task<IActionResult> Details(string invitationId)
    {
        if (string.IsNullOrEmpty(HttpContext.User.FindFirstValue(ControllerConstants.UserRefClaimKeyName)))
        {
            return RedirectToAction(ControllerConstants.IndexActionName, ControllerConstants.HomeControllerName);
        }

        var invitation = await _invitationOrchestrator.GetInvitation(invitationId);

        return View(invitation);
    }

    [HttpPost]
    [Authorize]
    [Route("accept", Name = RouteNames.InvitationAcceptPost)]
    public async Task<IActionResult> Accept(long invitation, UserInvitationsViewModel model)
    {
        if (string.IsNullOrEmpty(HttpContext.User.FindFirstValue(ControllerConstants.UserRefClaimKeyName)))
        {
            return RedirectToAction(ControllerConstants.IndexActionName, ControllerConstants.HomeControllerName);
        }

        var invitationItem = model.Invitations.SingleOrDefault(c => c.Id == invitation);

        if (invitationItem == null)
        {
            return RedirectToAction(ControllerConstants.IndexActionName, ControllerConstants.HomeControllerName);
        }

        await _invitationOrchestrator.AcceptInvitation(invitationItem.Id, HttpContext.User.FindFirstValue(ControllerConstants.UserRefClaimKeyName));

        var flashMessage = new FlashMessageViewModel
        {
            Headline = "Invitation accepted",
            Message = $"You can now access the {invitationItem.AccountName} account",
            Severity = FlashMessageSeverityLevel.Success
        };
        AddFlashMessageToCookie(flashMessage);


        return RedirectToAction(ControllerConstants.IndexActionName, ControllerConstants.HomeControllerName);
    }

    [HttpPost]
    [Authorize]
    [Route("create")]
    public async Task<IActionResult> Create(InviteTeamMemberViewModel model)
    {
        if (string.IsNullOrEmpty(HttpContext.User.FindFirstValue(ControllerConstants.UserRefClaimKeyName)))
        {
            return RedirectToAction(ControllerConstants.IndexActionName, ControllerConstants.HomeControllerName);
        }

        await _invitationOrchestrator.CreateInvitation(model, HttpContext.User.FindFirstValue(ControllerConstants.UserRefClaimKeyName));

        return RedirectToAction(ControllerConstants.IndexActionName, ControllerConstants.HomeControllerName);
    }

    [HttpGet]
    [Route("register-and-accept")]
    public IActionResult AcceptInvitationNewUser()
    {
        var schema = HttpContext?.Request.Scheme;
        var authority = HttpContext?.Request.Host;
        var appConstants = new Constants(_configuration.Identity);
        return new RedirectResult($"{appConstants.RegisterLink()}{schema}://{authority}/invitations");
    }


    [HttpGet]
    [Route("accept")]
    public IActionResult AcceptInvitationExistingUser()
    {
        return RedirectToAction("All");
    }
}