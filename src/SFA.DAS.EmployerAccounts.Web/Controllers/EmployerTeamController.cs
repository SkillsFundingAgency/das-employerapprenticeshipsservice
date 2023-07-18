using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using SFA.DAS.Employer.Shared.UI;
using SFA.DAS.Employer.Shared.UI.Attributes;
using SFA.DAS.EmployerAccounts.Web.Authentication;
using SFA.DAS.EmployerAccounts.Web.RouteValues;

namespace SFA.DAS.EmployerAccounts.Web.Controllers;

[SetNavigationSection(NavigationSection.AccountsTeamsView)]
[Route("accounts/{HashedAccountId}/teams")]
public class EmployerTeamController : BaseController
{
    private readonly IUrlActionHelper _urlActionHelper;
    private readonly EmployerTeamOrchestratorWithCallToAction _employerTeamOrchestrator;

    public EmployerTeamController(
        ICookieStorageService<FlashMessageViewModel> flashMessage,
        EmployerTeamOrchestratorWithCallToAction employerTeamOrchestrator,
        IUrlActionHelper urlActionHelper)
        : base(flashMessage)
    {
        _employerTeamOrchestrator = employerTeamOrchestrator;
        _urlActionHelper = urlActionHelper;
    }

    [HttpGet]
    [SetNavigationSection(NavigationSection.AccountsHome)]
    [Authorize(Policy = nameof(PolicyNames.HasEmployerViewerTransactorOwnerAccount))]
    [Route("", Name = RouteNames.EmployerTeamIndex)]
    public async Task<IActionResult> Index(string hashedAccountId)
    {
        try
        {
            PopulateViewBagWithExternalUserId();
            var response = await GetAccountInformation(hashedAccountId);

            if (response.Status != HttpStatusCode.OK)
            {
                return View(response);
            }

            if (!response.Data.Account.NameConfirmed)
            {
                return RedirectToRoute(RouteNames.ContinueNewEmployerAccountTaskList, new { hashedAccountId } );
            }

            return View(response);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
        
    }

    [HttpGet]
    [Route("view", Name = RouteNames.EmployerTeamView)]
    [Authorize(Policy = nameof(PolicyNames.HasEmployerViewerTransactorOwnerAccountOrSupport))]
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
    [Authorize(Policy = nameof(PolicyNames.HasEmployerViewerTransactorOwnerAccount))]
    public IActionResult AddedProvider([FromRoute] string hashedAccountId, [FromRoute] string providerName)
    {
        AddFlashMessageToCookie(new FlashMessageViewModel
        {
            Headline = "Your account has been created",
            Message = $"You account has been created and you've successfully updated permissions for {Uri.UnescapeDataString(providerName).ToUpper()}",
            Severity = FlashMessageSeverityLevel.Success
        });

        return RedirectToRoute(RouteNames.EmployerTeamIndex, new { hashedAccountId });
    }

    [HttpGet]
    [Route("invite", Name = RouteNames.EmployerTeamInvite)]
    [Authorize(Policy = nameof(PolicyNames.HasEmployerViewerTransactorOwnerAccountOrSupport))]
    public async Task<IActionResult> Invite(string hashedAccountId)
    {
        var response = await _employerTeamOrchestrator.GetNewInvitation(hashedAccountId, HttpContext.User.FindFirstValue(ControllerConstants.UserRefClaimKeyName));

        return View(response);
    }

    [HttpPost]
    [Route("invite", Name = RouteNames.EmployerTeamInvitePost)]
    [Authorize(Policy = nameof(PolicyNames.HasEmployerViewerTransactorOwnerAccountOrSupport))]
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

            return RedirectToAction(ControllerConstants.NextStepsActionName, new { model.HashedAccountId });
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
    [Authorize(Policy = nameof(PolicyNames.HasEmployerViewerTransactorOwnerAccountOrSupport))]
    public IActionResult NextSteps(string hashedAccountId)
    {
        var model = new OrchestratorResponse<InviteTeamMemberNextStepsViewModel>
        {
            FlashMessage = GetFlashMessageViewModelFromCookie(),
            Data = new InviteTeamMemberNextStepsViewModel
            {
                HashedAccountId = hashedAccountId
            }
        };

        return View(model);
    }

    [HttpPost]
    [Route("invite/next", Name = RouteNames.EmployerTeamInviteNextPost)]
    [Authorize(Policy = nameof(PolicyNames.HasEmployerViewerTransactorOwnerAccountOrSupport))]
    public IActionResult NextSteps(int? choice, string hashedAccountId)
    {
        switch (choice ?? 0)
        {
            case 1: return RedirectToAction(ControllerConstants.InviteActionName, new { hashedAccountId });
            case 2: return RedirectToAction(ControllerConstants.ViewTeamActionName, new { hashedAccountId });
            case 3: return RedirectToAction(ControllerConstants.IndexActionName, new { hashedAccountId });
            default:
                var model = new OrchestratorResponse<InviteTeamMemberNextStepsViewModel>
                {
                    FlashMessage = GetFlashMessageViewModelFromCookie(),
                    Data = new InviteTeamMemberNextStepsViewModel
                    {
                        ErrorMessage = "You must select an option to continue.",
                    }
                };
                return View(model); //No option entered
        }
    }

    [HttpGet]
    [Route("{hashedInvitationId}/cancel", Name = RouteNames.EmployerTeamCancelInvitation)]
    [Authorize(Policy = nameof(PolicyNames.HasEmployerViewerTransactorOwnerAccountOrSupport))]
    public async Task<IActionResult> Cancel(string hashedInvitationId)
    {
        var invitation = await _employerTeamOrchestrator.GetInvitation(hashedInvitationId);

        return View(invitation);
    }

    [HttpPost]
    [Route("{hashedInvitationId}/cancel", Name = RouteNames.EmployerTeamCancelInvitationPost)]
    [Authorize(Policy = nameof(PolicyNames.HasEmployerViewerTransactorOwnerAccountOrSupport))]
    public async Task<IActionResult> Cancel(string hashedInvitationId, string email, string hashedAccountId, int cancel)
    {
        if (cancel != 1)
            return RedirectToAction(ControllerConstants.ViewTeamViewName, new { HashedAccountId = hashedAccountId });

        var response = await _employerTeamOrchestrator.Cancel(email, hashedAccountId, HttpContext.User.FindFirstValue(ControllerConstants.UserRefClaimKeyName));

        return View(ControllerConstants.ViewTeamViewName, response);
    }

    [HttpPost]
    [Route("resend", Name = RouteNames.EmployerTeamResendInvite)]
    [Authorize(Policy = nameof(PolicyNames.HasEmployerViewerTransactorOwnerAccountOrSupport))]
    public async Task<IActionResult> Resend(string hashedAccountId, string email, string name)
    {
        var response = await _employerTeamOrchestrator.Resend(email, hashedAccountId, HttpContext.User.FindFirstValue(ControllerConstants.UserRefClaimKeyName), name);

        return View(ControllerConstants.ViewTeamViewName, response);
    }

    [HttpGet]
    [Route("{email}/remove", Name = RouteNames.RemoveTeamMember)]
    [Authorize(Policy = nameof(PolicyNames.HasEmployerViewerTransactorOwnerAccountOrSupport))]
    public async Task<IActionResult> Remove(string hashedAccountId, string email)
    {
        var response = await _employerTeamOrchestrator.Review(hashedAccountId, email);

        return View(response);
    }

    [HttpPost]
    [Route("{email}/remove", Name = RouteNames.ConfirmRemoveTeamMember)]
    [Authorize(Policy = nameof(PolicyNames.HasEmployerViewerTransactorOwnerAccountOrSupport))]
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
    [Route("{email}/role/change", Name = RouteNames.EmployerTeamGetChangeRole)]
    [Authorize(Policy = nameof(PolicyNames.HasEmployerViewerTransactorOwnerAccountOrSupport))]
    public async Task<IActionResult> ChangeRole(string hashedAccountId, string email)
    {
        var teamMember = await _employerTeamOrchestrator.GetTeamMemberWhetherActiveOrNot(hashedAccountId, email, HttpContext.User.FindFirstValue(ControllerConstants.UserRefClaimKeyName));

        return View(teamMember);
    }

    [HttpPost]
    [Route("{email}/role/change", Name = RouteNames.EmployerTeamChangeRolePost)]
    [Authorize(Policy = nameof(PolicyNames.HasEmployerViewerTransactorOwnerAccountOrSupport))]
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
    [Route("{email}/review", Name = RouteNames.EmployerTeamReview)]
    [Authorize(Policy = nameof(PolicyNames.HasEmployerViewerTransactorOwnerAccountOrSupport))]
    public async Task<IActionResult> Review(string hashedAccountId, string email)
    {
        var invitation = await _employerTeamOrchestrator.GetTeamMemberWhetherActiveOrNot(hashedAccountId, email, HttpContext.User.FindFirstValue(ControllerConstants.UserRefClaimKeyName));

        return View(invitation);
    }

    [HttpGet]
    [Route("continuesetupcreateadvert", Name = RouteNames.CreateAdvert)]
    [Authorize(Policy = nameof(PolicyNames.HasEmployerViewerTransactorOwnerAccount))]
    public IActionResult ContinueSetupCreateAdvert(string hashedAccountId)
    {
        return View();
    }

    [HttpPost]
    [Route("continuesetupcreateadvert", Name = RouteNames.CreateAdvertPost)]
    [Authorize(Policy = nameof(PolicyNames.HasEmployerViewerTransactorOwnerAccount))]
    public IActionResult ContinueSetupCreateAdvert(string hashedAccountId, bool? requiresAdvert)
    {
        if (!requiresAdvert.HasValue)
        {
            ViewData.ModelState.AddModelError("Choice", "You must select an option to continue.");
            return View();
        }

        if (requiresAdvert.Value)
        {
            return Redirect(_urlActionHelper.EmployerRecruitAction());
        }

        return Redirect(_urlActionHelper.EmployerCommitmentsV2Action("unapproved/Inform"));
    }

    [HttpGet]
    [Route("triagewhichcourseyourapprenticewilltake", Name = RouteNames.TriageCourse)]
    [Authorize(Policy = nameof(PolicyNames.HasEmployerViewerTransactorOwnerAccount))]
    public IActionResult TriageWhichCourseYourApprenticeWillTake()
    {
        return View();
    }

    [HttpPost]
    [Route("triagewhichcourseyourapprenticewilltake", Name = RouteNames.TriageCoursePost)]
    [Authorize(Policy = nameof(PolicyNames.HasEmployerViewerTransactorOwnerAccount))]
    public IActionResult TriageWhichCourseYourApprenticeWillTake(string hashedAccountId, TriageViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        switch (model.TriageOption)
        {
            case TriageOptions.Yes:
                {
                    return RedirectToRoute(RouteNames.TriageChosenProvider, new { hashedAccountId });
                }

            case TriageOptions.No:
                {
                    return RedirectToRoute(RouteNames.TriageCannotSetupWithoutChosenCourseAndProvider, new { hashedAccountId });
                }

            default:
                {
                    return View(model);
                }
        }
    }

    [HttpGet]
    [Route("triageyoucannotsetupanapprenticeshipyetcourseprovider", Name = RouteNames.TriageCannotSetupWithoutChosenCourseAndProvider)]
    [Authorize(Policy = nameof(PolicyNames.HasEmployerViewerTransactorOwnerAccount))]
    public IActionResult TriageYouCannotSetupAnApprenticeshipYetCourseProvider()
    {
        return View();
    }

    [HttpGet]
    [Route("triagehaveyouchosenatrainingprovider", Name = RouteNames.TriageChosenProvider)]
    [Authorize(Policy = nameof(PolicyNames.HasEmployerViewerTransactorOwnerAccount))]
    public IActionResult TriageHaveYouChosenATrainingProvider()
    {
        return View();
    }

    [HttpPost]
    [Route("triagehaveyouchosenatrainingprovider", Name = RouteNames.TriageChosenProviderPost)]
    [Authorize(Policy = nameof(PolicyNames.HasEmployerViewerTransactorOwnerAccount))]
    public IActionResult TriageHaveYouChosenATrainingProvider(string hashedAccountId, TriageViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        switch (model.TriageOption)
        {
            case TriageOptions.Yes:
                {
                    return RedirectToRoute(RouteNames.TriageWhenWillApprenticeshipStart, new { hashedAccountId });
                }

            case TriageOptions.No:
                {
                    return RedirectToRoute(RouteNames.TriageCannotSetupWithoutChosenProvider, new { hashedAccountId });
                }

            default:
                {
                    return View(model);
                }
        }
    }

    [HttpGet]
    [Route("triageyoucannotsetupanapprenticeshipyetprovider", Name = RouteNames.TriageCannotSetupWithoutChosenProvider)]
    [Authorize(Policy = nameof(PolicyNames.HasEmployerViewerTransactorOwnerAccount))]
    public IActionResult TriageYouCannotSetupAnApprenticeshipYetProvider()
    {
        return View();
    }

    [HttpGet]
    [Route("triagewillapprenticeshiptrainingstart", Name = RouteNames.TriageWhenWillApprenticeshipStart)]
    [Authorize(Policy = nameof(PolicyNames.HasEmployerViewerTransactorOwnerAccount))]
    public IActionResult TriageWillApprenticeshipTrainingStart()
    {
        return View();
    }

    [HttpPost]
    [Route("triagewillapprenticeshiptrainingstart", Name = RouteNames.TriageWhenWillApprenticeshipStartPost)]
    [Authorize(Policy = nameof(PolicyNames.HasEmployerViewerTransactorOwnerAccount))]
    public IActionResult TriageWillApprenticeshipTrainingStart(string hashedAccountId,TriageViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        switch (model.TriageOption)
        {
            case TriageOptions.Yes:
                {
                    return RedirectToRoute(RouteNames.TriageWhenApprenticeshipForExistingEmployee, new { hashedAccountId });
                }

            case TriageOptions.No:
                {
                    return RedirectToRoute(RouteNames.TriageCannotSetupWithoutStartDate, new { hashedAccountId });
                }

            case TriageOptions.Unknown:
                {
                    return RedirectToRoute(RouteNames.TriageCannotSetupWithoutApproximateStartDate, new { hashedAccountId });
                }

            default:
                {
                    return View(model);
                }
        }
    }

    [HttpGet]
    [Route("triageyoucannotsetupanapprenticeshipyetstartdate", Name = RouteNames.TriageCannotSetupWithoutStartDate)]
    [Authorize(Policy = nameof(PolicyNames.HasEmployerViewerTransactorOwnerAccount))]
    public IActionResult TriageYouCannotSetupAnApprenticeshipYetStartDate()
    {
        return View();
    }

    [HttpGet]
    [Route("triageyoucannotsetupanapprenticeshipyetapproximatestartdate", Name = RouteNames.TriageCannotSetupWithoutApproximateStartDate)]
    [Authorize(Policy = nameof(PolicyNames.HasEmployerViewerTransactorOwnerAccount))]
    public IActionResult TriageYouCannotSetupAnApprenticeshipYetApproximateStartDate()
    {
        return View();
    }

    [HttpGet]
    [Route("triageapprenticeforexistingemployee", Name = RouteNames.TriageWhenApprenticeshipForExistingEmployee)]
    [Authorize(Policy = nameof(PolicyNames.HasEmployerViewerTransactorOwnerAccount))]
    public IActionResult TriageApprenticeForExistingEmployee()
    {
        return View();
    }

    [HttpPost]
    [Route("triageapprenticeforexistingemployee", Name = RouteNames.TriageWhenApprenticeshipForExistingEmployeePost)]
    [Authorize(Policy = nameof(PolicyNames.HasEmployerViewerTransactorOwnerAccount))]
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
        {
            ViewBag.UserId = externalUserId;
        }
    }
}