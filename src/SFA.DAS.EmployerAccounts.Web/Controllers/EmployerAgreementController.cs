using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using SFA.DAS.Employer.Shared.UI;
using SFA.DAS.Employer.Shared.UI.Attributes;
using SFA.DAS.EmployerAccounts.Web.Authentication;
using SFA.DAS.EmployerAccounts.Web.RouteValues;

namespace SFA.DAS.EmployerAccounts.Web.Controllers;

[Route("accounts/{HashedAccountId}")]
[Authorize(Policy = nameof(PolicyNames.HasEmployerViewerTransactorOwnerAccount))]
[SetNavigationSection(NavigationSection.AccountsAgreements)]
public class EmployerAgreementController : BaseController
{
    private const int InvitationComplete = 4;
    private readonly EmployerAgreementOrchestrator _orchestrator;
    private readonly IMediator _mediator;
    private readonly IUrlActionHelper _urlActionHelper;
    private const int ViewAgreementNow = 1;
    private const int ViewAgreementLater = 2;

    public EmployerAgreementController(
        EmployerAgreementOrchestrator orchestrator,
        ICookieStorageService<FlashMessageViewModel> flashMessage,
        IMediator mediator,
        IUrlActionHelper urlActionHelper)
        : base( flashMessage)
    {
        _orchestrator = orchestrator;
        _mediator = mediator;
        _urlActionHelper = urlActionHelper;
    }

    [HttpGet]
    [Route("agreements", Name = RouteNames.EmployerAgreementIndex)]
    public async Task<IActionResult> Index(string hashedAccountId, bool agreementSigned = false)
    {
        var model = await _orchestrator.Get(hashedAccountId, HttpContext.User.FindFirstValue(ControllerConstants.UserRefClaimKeyName));

        var flashMessage = GetFlashMessageViewModelFromCookie();
        if (flashMessage != null)
        {
            model.FlashMessage = flashMessage;
        }

        ViewBag.ShowConfirmation = agreementSigned && model.Data.EmployerAgreementsData.HasPendingAgreements;

        return View(model);
    }

    [HttpGet]
    [Route("agreements/{hashedAgreementId}/details")]
    public async Task<IActionResult> Details(string hashedAccountId, string hashedAgreementId)
    {
        var agreement = await _orchestrator.GetById(
            hashedAgreementId,
            hashedAccountId,
            HttpContext.User.FindFirstValue(ControllerConstants.UserRefClaimKeyName)
        );

        return View(agreement);
    }

    [HttpGet]
    [Route("agreements/{hashedAgreementId}/view", Name = RouteNames.AgreementView)]
    public async Task<IActionResult> View(string hashedAccountId, string hashedAgreementId)
    {
        var agreement = await _orchestrator.GetSignedAgreementViewModel(hashedAccountId, hashedAgreementId, HttpContext.User.FindFirstValue(ControllerConstants.UserRefClaimKeyName));
        return View(agreement.Data);
    }

    [HttpGet]
    [Route("agreements/unsigned/view")]
    public async Task<IActionResult> ViewUnsignedAgreements(string hashedAccountId)
    {
        var unsignedAgeementResponse = await _orchestrator.GetNextUnsignedAgreement(hashedAccountId, HttpContext.User.FindFirstValue(ControllerConstants.UserRefClaimKeyName));

        if (!unsignedAgeementResponse.Data.HasNextAgreement) return RedirectToAction(ControllerConstants.IndexActionName);

        return RedirectToAction(ControllerConstants.AboutYourAgreementActionName, new { agreementId = unsignedAgeementResponse.Data.NextAgreementHashedId });
    }

    [HttpGet]
    [Route("agreements/{hashedAgreementId}/about-your-agreement", Name = RouteNames.AboutYourAgreement)]
    public async Task<ViewResult> AboutYourAgreement(string hashedAccountId, string hashedAgreementId)
    {
        var agreement = await _orchestrator.GetById(
            hashedAgreementId,
            hashedAccountId,
            HttpContext.User.FindFirstValue(ControllerConstants.UserRefClaimKeyName));

        var view = View(agreement);
        return view;
    }

    [HttpGet]
    [Route("agreements/{hashedAgreementId}/sign-your-agreement", Name = RouteNames.EmployerAgreementSignYourAgreement)]
    public async Task<IActionResult> SignAgreement(string hashedAccountId, string hashedAgreementId)
    {
        var externalUserId = HttpContext.User.FindFirstValue(ControllerConstants.UserRefClaimKeyName);

        var viewModel = await _orchestrator.GetSignedAgreementViewModel(hashedAccountId, hashedAgreementId, externalUserId);

        return View(viewModel.Data);
    }

    [HttpPost]
    [Route("agreements/{hashedAgreementId}/sign", Name = RouteNames.EmployerAgreementSign)]
    public async Task<IActionResult> Sign( string hashedAccountId, string hashedAgreementId, int? choice)
    {
        var userInfo = HttpContext.User.FindFirstValue(ControllerConstants.UserRefClaimKeyName);

        if (choice == null)
        {
            var agreement = await _orchestrator.GetSignedAgreementViewModel(hashedAccountId, hashedAgreementId, userInfo);

            ModelState.AddModelError(nameof(agreement.Data.Choice), "Select whether you accept the agreement");

            return View(ControllerConstants.SignAgreementViewName, agreement.Data);
        }

        if (choice == SignEmployerAgreementViewModel.ReviewAgreementLater)
        {
            return RedirectToRoute(RouteNames.EmployerTeamIndex, new { hashedAccountId });
        }

        var response = await _orchestrator.SignAgreement(hashedAgreementId, hashedAccountId, userInfo, DateTime.UtcNow);

        if (response.Status == HttpStatusCode.Unauthorized)
        {
            return View(response);
        }

        var user = await _mediator.Send(new GetUserByRefQuery { UserRef = userInfo });

        if (!string.IsNullOrWhiteSpace(user.User.CorrelationId))
        {
            var getProviderInvitationQueryResponse = await _mediator.Send(new GetProviderInvitationQuery
            {
                CorrelationId = Guid.Parse(user.User.CorrelationId)
            });

            if (getProviderInvitationQueryResponse.Result?.Status < InvitationComplete)
            {
                return Redirect(_urlActionHelper.ProviderRelationshipsAction($"providers/invitation/{user.User.CorrelationId}"));
            }
        }


        if (response.Status == HttpStatusCode.OK)
        {
            ViewBag.CompanyName = response.Data.LegalEntityName;
            ViewBag.HasFurtherPendingAgreements = response.Data.HasFurtherPendingAgreements;
            return View(ControllerConstants.AcceptedEmployerAgreementViewName);
        }

        return RedirectToAction(ControllerConstants.SignAgreementActionName, new GetEmployerAgreementRequest { HashedAgreementId = hashedAgreementId, ExternalUserId = userInfo, HashedAccountId = hashedAccountId });
    }

    [HttpGet]
    [Route("agreements/{hashedAgreementId}/agreement-pdf", Name = RouteNames.GetPdfAgreement)]
    public async Task<IActionResult> GetPdfAgreement(string hashedAccountId, string hashedAgreementId)
    {
        var stream = await _orchestrator.GetPdfEmployerAgreement(hashedAccountId, hashedAgreementId, HttpContext.User.FindFirstValue(ControllerConstants.UserRefClaimKeyName));

        if (stream.Data.PdfStream == null)
        {
            // ReSharper disable once Mvc.ViewNotResolved
            return View(stream);
        }

        return new FileStreamResult(stream.Data.PdfStream, ControllerConstants.PdfContentTypeName);
    }

    [HttpGet]
    [Route("agreements/{hashedAgreementId}/signed-agreement-pdf", Name = RouteNames.GetSignedPdfAgreement)]
    public async Task<IActionResult> GetSignedPdfAgreement(string hashedAccountId, string hashedAgreementId)
    {
        var stream = await _orchestrator.GetSignedPdfEmployerAgreement(hashedAccountId, hashedAgreementId, HttpContext.User.FindFirstValue(ControllerConstants.UserRefClaimKeyName));

        if (stream.Data.PdfStream == null)
        {
            // ReSharper disable once Mvc.ViewNotResolved
            return View(stream);
        }

        return new FileStreamResult(stream.Data.PdfStream, ControllerConstants.PdfContentTypeName);
    }

    [HttpGet]
    [Route("agreements/{accountLegalEntityHashedId}/remove")]
    public async Task<IActionResult> ConfirmRemoveOrganisation(string hashedAccountId, string accountLegalEntityHashedId)
    {
        var model = await _orchestrator.GetConfirmRemoveOrganisationViewModel(hashedAccountId, accountLegalEntityHashedId, HttpContext.User.FindFirstValue(ControllerConstants.UserRefClaimKeyName));

        return View(model.Data != null && model.Data.CanBeRemoved ? ControllerConstants.ConfirmRemoveOrganisationActionName : ControllerConstants.CannotRemoveOrganisationViewName, model);
    }

    [HttpPost]
    [Route("agreements/{accountLegalEntityHashedId}/remove", Name = RouteNames.PostConfirmRemoveOrganisation)]
    public async Task<IActionResult> RemoveOrganisation(ConfirmOrganisationToRemoveViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(ControllerConstants.ConfirmRemoveOrganisationViewName, new OrchestratorResponse<ConfirmOrganisationToRemoveViewModel> { Data = model });
        }
        if (!model.Remove.HasValue || !model.Remove.Value) return RedirectToRoute(RouteNames.EmployerAgreementIndex, new { model.HashedAccountId });

        var response = await _orchestrator.RemoveLegalAgreement(model, HttpContext.User.FindFirstValue(ControllerConstants.UserRefClaimKeyName));

        if (response.Status == HttpStatusCode.OK)
        {
            AddFlashMessageToCookie(response.FlashMessage);
            return RedirectToRoute(RouteNames.EmployerAgreementIndex, new { model.HashedAccountId });
        }

        AddFlashMessageToCookie(response.FlashMessage);
        return View(ControllerConstants.ConfirmRemoveOrganisationViewName, response);
    }

    [HttpGet]
    [Route("agreements/{hashedAgreementId}/whenDoYouWantToView")]
    public async Task<IActionResult> WhenDoYouWantToView(string hashedAccountId, string hashedAgreementId)
    {
        var userInfo = HttpContext.User.FindFirstValue(ControllerConstants.UserRefClaimKeyName);
        var agreement = await _orchestrator.GetById(hashedAgreementId, hashedAccountId, userInfo);

        return View(new WhenDoYouWantToViewViewModel { EmployerAgreement = agreement.Data.EmployerAgreement });
    }

    [HttpPost]
    [Route("agreements/{hashedAgreementId}/whenDoYouWantToView")]
    public async Task<IActionResult> WhenDoYouWantToView(int? choice, string hashedAccountId, string hashedAgreementId)
    {
        switch (choice ?? 0)
        {
            case ViewAgreementNow: return RedirectToRoute(RouteNames.EmployerAgreementSignYourAgreement, new { hashedAccountId, hashedAgreementId });
            case ViewAgreementLater: return RedirectToRoute(RouteNames.EmployerTeamIndex, new { hashedAccountId });
            default:
            {
                var userInfo = HttpContext.User.FindFirstValue(ControllerConstants.UserRefClaimKeyName);
                var agreement = await _orchestrator.GetById(hashedAgreementId, hashedAccountId, userInfo);
                return View(new WhenDoYouWantToViewViewModel { EmployerAgreement = agreement.Data.EmployerAgreement, InError = true });
            }
        }
    }

    [HttpGet]
    [Route("organisations/{accountLegalEntityHashedId}/agreements")]
    public async Task<IActionResult> ViewAllAgreements(string hashedAccountId, string accountLegalEntityHashedId)
    {
        var model = await _orchestrator.GetOrganisationAgreements(accountLegalEntityHashedId);
        return View(model.Data);
    }
}