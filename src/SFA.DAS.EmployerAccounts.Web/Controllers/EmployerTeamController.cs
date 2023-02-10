using System.Security.Claims;
using SFA.DAS.Authorization.Mvc.Attributes;
using SFA.DAS.EmployerAccounts.Helpers;
using SFA.DAS.Encoding;

namespace SFA.DAS.EmployerAccounts.Web.Controllers;

[DasAuthorize]
[Route("accounts/{HashedAccountId}/teams")]
public class EmployerTeamController : BaseController
{
    private readonly IUrlActionHelper _urlActionHelper;
    private readonly EmployerTeamOrchestrator _employerTeamOrchestrator;

    public EmployerTeamController(
        ICookieStorageService<FlashMessageViewModel> flashMessage,
        EmployerTeamOrchestrator employerTeamOrchestrator,
        IMultiVariantTestingService multiVariantTestingService)
        : base(flashMessage, multiVariantTestingService)
    {
        _employerTeamOrchestrator = employerTeamOrchestrator;
    }

    [HttpGet]
    public async Task<IActionResult> Index(string hashedAccountId)
    {
        PopulateViewBagWithExternalUserId();
        var response = await GetAccountInformation(hashedAccountId);

        if (response.Status != HttpStatusCode.OK)
        {
            return View(response);
        }

        if (!response.Data.HasPayeScheme)
        {
            ViewBag.HideNav = true;
        }

        return View(response);
    }

    [HttpGet]
    [Route("view")]
    public async Task<IActionResult> ViewTeam(string hashedAccountId)
    {
        var response = await _employerTeamOrchestrator.GetTeamMembers(hashedAccountId, HttpContext.User.FindFirstValue(ControllerConstants.UserRefClaimKeyName));

        var flashMessage = GetFlashMessageViewModelFromCookie();
        if (flashMessage != null)
        {
            response.FlashMessage = flashMessage;
        }

        return View(response);
    }

    [HttpGet]
    [Route("AddedProvider/{providerName}")]
    public async Task<IActionResult> AddedProvider(string providerName)
    {
        AddFlashMessageToCookie(new FlashMessageViewModel
        {
            Headline = "Your account has been created",
            Message = $"You account has been created and you've successfully updated permissions for {WebUtility.UrlDecode(providerName.ToUpper())}",
            Severity = FlashMessageSeverityLevel.Success
        });

        return RedirectToAction(ControllerConstants.IndexActionName);
    }

    [HttpGet]
    [Route("invite")]
    public async Task<IActionResult> Invite(string hashedAccountId)
    {
        var response = await _employerTeamOrchestrator.GetNewInvitation(hashedAccountId, HttpContext.User.FindFirstValue(ControllerConstants.UserRefClaimKeyName));

        return View(response);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [Route("invite")]
    public async Task<IActionResult> Invite(InviteTeamMemberViewModel model)
    {
        var response = await _employerTeamOrchestrator.InviteTeamMember(model, HttpContext.User.FindFirstValue(ControllerConstants.UserRefClaimKeyName));

        if (response.Status == HttpStatusCode.OK)
        {
            var flashMessage = new FlashMessageViewModel
            {
                HiddenFlashMessageInformation = "page-invite-team-member-sent",
                Severity = FlashMessageSeverityLevel.Success,
                Headline = "Invitation sent",
                Message = $"You've sent an invitation to <strong>{model.Email}</strong>"
            };
            AddFlashMessageToCookie(flashMessage);

            return RedirectToAction(ControllerConstants.NextStepsActionName);
        }


        model.ErrorDictionary = response.FlashMessage.ErrorMessages;
        var errorResponse = new OrchestratorResponse<InviteTeamMemberViewModel>
        {
            Data = model,
            FlashMessage = response.FlashMessage,
        };

        return View(errorResponse);
    }

    [HttpGet]
    [Route("invite/next")]
    public async Task<IActionResult> NextSteps(string hashedAccountId)
    {
        var userId = HttpContext.User.FindFirstValue(ControllerConstants.UserRefClaimKeyName);
        var userShownWizard = await _employerTeamOrchestrator.UserShownWizard(userId, hashedAccountId);

        var model = new OrchestratorResponse<InviteTeamMemberNextStepsViewModel>
        {
            FlashMessage = GetFlashMessageViewModelFromCookie(),
            Data = new InviteTeamMemberNextStepsViewModel
            {
                UserShownWizard = userShownWizard,
                HashedAccountId = hashedAccountId
            }
        };

        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [Route("invite/next")]
    public async Task<IActionResult> NextSteps(int? choice, string hashedAccountId)
    {
        var userId = HttpContext.User.FindFirstValue(ControllerConstants.UserRefClaimKeyName);
        var userShownWizard = await _employerTeamOrchestrator.UserShownWizard(userId, hashedAccountId);

        switch (choice ?? 0)
        {
            case 1: return RedirectToAction(ControllerConstants.InviteActionName);
            case 2: return RedirectToAction(ControllerConstants.ViewTeamActionName);
            case 3: return RedirectToAction(ControllerConstants.IndexActionName);
            default:
                var model = new OrchestratorResponse<InviteTeamMemberNextStepsViewModel>
                {
                    FlashMessage = GetFlashMessageViewModelFromCookie(),
                    Data = new InviteTeamMemberNextStepsViewModel
                    {
                        ErrorMessage = "You must select an option to continue.",
                        UserShownWizard = userShownWizard
                    }
                };
                return View(model); //No option entered
        }
    }

    [HttpGet]
    [Route("{invitationId}/cancel")]
    public async Task<IActionResult> Cancel(string email, string invitationId, string hashedAccountId)
    {
        var invitation = await _employerTeamOrchestrator.GetInvitation(invitationId);

        return View(invitation);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [Route("{invitationId}/cancel")]
    public async Task<IActionResult> Cancel(string invitationId, string email, string hashedAccountId, int cancel)
    {
        if (cancel != 1)
            return RedirectToAction(ControllerConstants.ViewTeamViewName, new { HashedAccountId = hashedAccountId });

        var response = await _employerTeamOrchestrator.Cancel(email, hashedAccountId, HttpContext.User.FindFirstValue(ControllerConstants.UserRefClaimKeyName));

        return View(ControllerConstants.ViewTeamViewName, response);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [Route("resend")]
    public async Task<IActionResult> Resend(string hashedAccountId, string email, string name)
    {
        var response = await _employerTeamOrchestrator.Resend(email, hashedAccountId, HttpContext.User.FindFirstValue(ControllerConstants.UserRefClaimKeyName), name);

        return View(ControllerConstants.ViewTeamViewName, response);
    }

    [HttpGet]
    [Route("{email}/remove")]
    public async Task<IActionResult> Remove(string hashedAccountId, string email)
    {
        var response = await _employerTeamOrchestrator.Review(hashedAccountId, email);

        return View(response);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [Route("{email}/remove")]
    public async Task<IActionResult> Remove(long userId, string hashedAccountId, string email, int remove)
    {
        Exception exception;
        HttpStatusCode httpStatusCode;

        try
        {
            if (remove != 1)
                return RedirectToAction(ControllerConstants.ViewTeamViewName, new { HashedAccountId = hashedAccountId });

            var response = await _employerTeamOrchestrator.Remove(userId, hashedAccountId, HttpContext.User.FindFirstValue(ControllerConstants.UserRefClaimKeyName));

            return View(ControllerConstants.ViewTeamViewName, response);
        }
        catch (InvalidRequestException e)
        {
            httpStatusCode = HttpStatusCode.BadRequest;
            exception = e;
        }
        catch (UnauthorizedAccessException e)
        {
            httpStatusCode = HttpStatusCode.Unauthorized;
            exception = e;
        }

        var errorResponse = await _employerTeamOrchestrator.Review(hashedAccountId, email);
        errorResponse.Status = httpStatusCode;
        errorResponse.Exception = exception;

        return View(errorResponse);
    }

    [HttpGet]
    [Route("{email}/role/change")]
    public async Task<IActionResult> ChangeRole(string hashedAccountId, string email)
    {
        var teamMember = await _employerTeamOrchestrator.GetTeamMemberWhetherActiveOrNot(hashedAccountId, email, HttpContext.User.FindFirstValue(ControllerConstants.UserRefClaimKeyName));

        return View(teamMember);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [Route("{email}/role/change")]
    public async Task<IActionResult> ChangeRole(string hashedAccountId, string email, Role role)
    {
        var response = await _employerTeamOrchestrator.ChangeRole(hashedAccountId, email, role, HttpContext.User.FindFirstValue(ControllerConstants.UserRefClaimKeyName));

        if (response.Status == HttpStatusCode.OK)
        {
            return View(ControllerConstants.ViewTeamViewName, response);
        }

        var teamMemberResponse = await _employerTeamOrchestrator.GetTeamMemberWhetherActiveOrNot(hashedAccountId, email, HttpContext.User.FindFirstValue(ControllerConstants.UserRefClaimKeyName));

        //We have to override flash message as the change role view has different model to view team view
        teamMemberResponse.FlashMessage = response.FlashMessage;
        teamMemberResponse.Exception = response.Exception;

        return View(teamMemberResponse);
    }

    [HttpGet]
    [Route("{email}/review")]
    public async Task<IActionResult> Review(string hashedAccountId, string email)
    {
        var invitation = await _employerTeamOrchestrator.GetTeamMemberWhetherActiveOrNot(hashedAccountId, email, HttpContext.User.FindFirstValue(ControllerConstants.UserRefClaimKeyName));

        return View(invitation);
    }

    [HttpGet]
    [Route("hideWizard")]
    public async Task<IActionResult> HideWizard(string hashedAccountId)
    {
        var externalUserId = HttpContext.User.FindFirstValue(ControllerConstants.UserRefClaimKeyName);

        await _employerTeamOrchestrator.HideWizard(hashedAccountId, externalUserId);

        return RedirectToAction(ControllerConstants.IndexActionName);
    }

    [HttpGet]
    [Route("continuesetupcreateadvert")]
    public IActionResult ContinueSetupCreateAdvert(string hashedAccountId)
    {
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [Route("continuesetupcreateadvert")]
    public IActionResult ContinueSetupCreateAdvert(string hashedAccountId, bool? requiresAdvert)
    {
        if (!requiresAdvert.HasValue)
        {
            ViewData.ModelState.AddModelError("Choice", "You must select an option to continue.");
            return View();
        }
        else if (requiresAdvert.Value == true)
        {
            return Redirect(_urlActionHelper.EmployerRecruitAction());
        }

        return Redirect(_urlActionHelper.EmployerCommitmentsV2Action("unapproved/Inform"));
    }

    [HttpGet]
    [Route("triagewhichcourseyourapprenticewilltake")]
    public IActionResult TriageWhichCourseYourApprenticeWillTake()
    {
        return View();
    }

    [HttpPost]
    [Route("triagewhichcourseyourapprenticewilltake")]
    [ValidateAntiForgeryToken]
    public IActionResult TriageWhichCourseYourApprenticeWillTake(TriageViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        switch (model.TriageOption)
        {
            case TriageOptions.Yes:
                {
                    return RedirectToAction(ControllerConstants.TriageHaveYouChosenATrainingProviderActionName);
                }

            case TriageOptions.No:
                {
                    return RedirectToAction(ControllerConstants.TriageYouCannotSetupAnApprenticeshipYetCourseProviderActionName);
                }

            default:
                {
                    return View(model);
                }
        }
    }

    [HttpGet]
    [Route("triageyoucannotsetupanapprenticeshipyetcourseprovider")]
    public IActionResult TriageYouCannotSetupAnApprenticeshipYetCourseProvider()
    {
        return View();
    }

    [HttpGet]
    [Route("triagehaveyouchosenatrainingprovider")]
    public IActionResult TriageHaveYouChosenATrainingProvider()
    {
        return View();
    }

    [HttpPost]
    [Route("triagehaveyouchosenatrainingprovider")]
    [ValidateAntiForgeryToken]
    public IActionResult TriageHaveYouChosenATrainingProvider(TriageViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        switch (model.TriageOption)
        {
            case TriageOptions.Yes:
                {
                    return RedirectToAction(ControllerConstants.TriageWillApprenticeshipTrainingStartActionName);
                }

            case TriageOptions.No:
                {
                    return RedirectToAction(ControllerConstants.TriageYouCannotSetupAnApprenticeshipYetProviderActionName);
                }

            default:
                {
                    return View(model);
                }
        }
    }

    [HttpGet]
    [Route("triageyoucannotsetupanapprenticeshipyetprovider")]
    public IActionResult TriageYouCannotSetupAnApprenticeshipYetProvider()
    {
        return View();
    }

    [HttpGet]
    [Route("triagewillapprenticeshiptrainingstart")]
    public IActionResult TriageWillApprenticeshipTrainingStart()
    {
        return View();
    }

    [HttpPost]
    [Route("triagewillapprenticeshiptrainingstart")]
    [ValidateAntiForgeryToken]
    public IActionResult TriageWillApprenticeshipTrainingStart(TriageViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        switch (model.TriageOption)
        {
            case TriageOptions.Yes:
                {
                    return RedirectToAction(ControllerConstants.TriageApprenticeForExistingEmployeeActionName);
                }

            case TriageOptions.No:
                {
                    return RedirectToAction(ControllerConstants.TriageYouCannotSetupAnApprenticeshipYetStartDateActionName);
                }

            case TriageOptions.Unknown:
                {
                    return RedirectToAction(ControllerConstants.TriageYouCannotSetupAnApprenticeshipYetApproximateStartDateActionName);
                }

            default:
                {
                    return View(model);
                }
        }
    }

    [HttpGet]
    [Route("triageyoucannotsetupanapprenticeshipyetstartdate")]
    public IActionResult TriageYouCannotSetupAnApprenticeshipYetStartDate()
    {
        return View();
    }

    [HttpGet]
    [Route("triageyoucannotsetupanapprenticeshipyetapproximatestartdate")]
    public IActionResult TriageYouCannotSetupAnApprenticeshipYetApproximateStartDate()
    {
        return View();
    }

    [HttpGet]
    [Route("triageapprenticeforexistingemployee")]
    public IActionResult TriageApprenticeForExistingEmployee()
    {
        return View();
    }

    [HttpPost]
    [Route("triageapprenticeforexistingemployee")]
    [ValidateAntiForgeryToken]
    public IActionResult TriageApprenticeForExistingEmployee(TriageViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        switch (model.TriageOption)
        {
            case TriageOptions.Yes:
                {
                    return View(ControllerConstants.TriageSetupApprenticeshipExistingEmployeeViewName);
                }

            case TriageOptions.No:
                {
                    return View(ControllerConstants.TriageSetupApprenticeshipNewEmployeeViewName);
                }

            default:
                {
                    return View(model);
                }
        }
    }

    public override IActionResult SupportUserBanner(IAccountIdentifier model = null)
    {
        Account account = null;

        if (model != null && model.HashedAccountId != null)
        {
            var externalUserId = HttpContext.User.Claims.First(x=> x.Type.Equals(ControllerConstants.UserRefClaimKeyName)).Value;
            var response = AsyncHelper.RunSync(() => _employerTeamOrchestrator.GetAccountSummary(model.HashedAccountId, externalUserId));
            account = response.Status != HttpStatusCode.OK ? null : response.Data.Account;
        }

        var consoleUserType = HttpContext.User.Claims.First(x => x.Type.Equals(ClaimTypes.Role)).Value == "Tier2User" ? "Service user (T2 Support)" : "Standard user";

        return ViewComponent("SupportUserBanner", new SupportUserBannerViewModel
        {
            Account = account,
            ConsoleUserType = consoleUserType
        });
    }

    private async Task<OrchestratorResponse<AccountDashboardViewModel>> GetAccountInformation(string hashedAccountId)
    {
        var externalUserId = HttpContext.User.FindFirstValue(ControllerConstants.UserRefClaimKeyName);
        var response = await _employerTeamOrchestrator.GetAccount(hashedAccountId, externalUserId);

        var flashMessage = GetFlashMessageViewModelFromCookie();

        if (flashMessage != null)
        {
            response.FlashMessage = flashMessage;
            response.Data.EmployerAccountType = flashMessage.HiddenFlashMessageInformation;
        }

        return response;
    }

    private void PopulateViewBagWithExternalUserId()
    {
        var externalUserId = HttpContext.User.FindFirstValue(ControllerConstants.UserRefClaimKeyName);
        if (externalUserId != null)
            ViewBag.UserId = externalUserId;
    }
}