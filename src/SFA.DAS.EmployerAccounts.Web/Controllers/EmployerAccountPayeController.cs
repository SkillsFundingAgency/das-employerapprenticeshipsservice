using System.Security.Claims;
using SFA.DAS.Authorization.Mvc.Attributes;
using SFA.DAS.EmployerAccounts.Web.Extensions;

namespace SFA.DAS.EmployerAccounts.Web.Controllers;

[DasAuthorize]
[Route("accounts")]
public class EmployerAccountPayeController : BaseController
{
    private readonly EmployerAccountPayeOrchestrator _employerAccountPayeOrchestrator;

    public EmployerAccountPayeController(
        EmployerAccountPayeOrchestrator employerAccountPayeOrchestrator,
        ICookieStorageService<FlashMessageViewModel> flashMessage,
        IMultiVariantTestingService multiVariantTestingService) 
        : base( flashMessage, multiVariantTestingService)
    {
        _employerAccountPayeOrchestrator = employerAccountPayeOrchestrator;
    }

    [HttpGet]
    [Route("{HashedAccountId}/schemes")]
    public async Task<IActionResult> Index(string hashedAccountId)
    {
        var model = await _employerAccountPayeOrchestrator.Get(hashedAccountId, HttpContext.User.FindFirstValue(ControllerConstants.UserRefClaimKeyName));

        var flashMessage = GetFlashMessageViewModelFromCookie();
        if (flashMessage != null)
        {
            model.FlashMessage = flashMessage;
        }

        return View(model);
    }

    [HttpGet]
    [Route("{HashedAccountId}/schemes/next")]
    public async Task<IActionResult> NextSteps(string hashedAccountId)
    {
        var model = await _employerAccountPayeOrchestrator.GetNextStepsViewModel(HttpContext.User.FindFirstValue(ControllerConstants.UserRefClaimKeyName), hashedAccountId);

        model.FlashMessage = GetFlashMessageViewModelFromCookie();

        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [Route("{HashedAccountId}/schemes/next")]
    public IActionResult NextSteps(int? choice)
    {
        switch (choice ?? 0)
        {
            case 1: return RedirectToAction(ControllerConstants.GatewayInformActionName);
            case 2: return RedirectToAction(ControllerConstants.IndexActionName, ControllerConstants.EmployerAccountTransactionsControllerName);
            case 3: return RedirectToAction(ControllerConstants.IndexActionName, ControllerConstants.EmployerTeamActionName);
            default:
                var model = new OrchestratorResponse<PayeSchemeNextStepsViewModel>
                {
                    FlashMessage = GetFlashMessageViewModelFromCookie(),
                    Data = new PayeSchemeNextStepsViewModel { ErrorMessage = "You must select an option to continue." }
                };
                return View(model); //No option entered
        }
    }

    [HttpGet]
    [Route("{HashedAccountId}/schemes/{empRef}/details")]
    public async Task<IActionResult> Details(string hashedAccountId, string empRef)
    {
        empRef = empRef.FormatPayeFromUrl();

        var response = await _employerAccountPayeOrchestrator.GetPayeDetails(empRef, hashedAccountId, HttpContext.User.FindFirstValue(ControllerConstants.UserRefClaimKeyName));

        return View(response);
    }

    [HttpGet]
    [Route("{HashedAccountId}/schemes/gatewayInform")]
    public async Task<IActionResult> GatewayInform(string hashedAccountId)
    {
        var response = await _employerAccountPayeOrchestrator.CheckUserIsOwner(
            hashedAccountId,
            HttpContext.User.FindFirstValue(ControllerConstants.EmailClaimKeyName),
            Url.Action(ControllerConstants.IndexActionName, ControllerConstants.EmployerAccountPayeControllerName, new { hashedAccountId }),
            Url.Action(ControllerConstants.GetGatewayActionName, ControllerConstants.EmployerAccountPayeControllerName, new { hashedAccountId }));

        return View(response);
    }

    [HttpGet]
    [Route("{HashedAccountId}/schemes/gateway")]
    public async Task<IActionResult> GetGateway(string hashedAccountId)
    {
        var url = await _employerAccountPayeOrchestrator.GetGatewayUrl(
            Url.Action(
                ControllerConstants.ConfirmPayeSchemeActionName,
                ControllerConstants.EmployerAccountPayeControllerName,
                null,
                HttpContext.Request.Scheme));

        return Redirect(url);
    }

    [HttpGet]
    [Route("{HashedAccountId}/schemes/confirm")]
    public async Task<IActionResult> ConfirmPayeScheme(string hashedAccountId)
    {
        var gatewayResponseModel = await _employerAccountPayeOrchestrator.GetPayeConfirmModel(
            hashedAccountId, 
            Request.Query[ControllerConstants.CodeKeyName], 
            Url.Action(ControllerConstants.ConfirmPayeSchemeActionName, ControllerConstants.EmployerAccountPayeControllerName, new { hashedAccountId }, Request.Scheme), 
            Request.Query);

        if (gatewayResponseModel.Status == HttpStatusCode.NotAcceptable)
        {
            gatewayResponseModel.Status = HttpStatusCode.OK;

            var model = await _employerAccountPayeOrchestrator.Get(hashedAccountId, HttpContext.User.FindFirstValue(ControllerConstants.UserRefClaimKeyName));
            model.FlashMessage = gatewayResponseModel.FlashMessage;

            return View(ControllerConstants.IndexActionName, model);
        }

        return View(gatewayResponseModel);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [Route("{HashedAccountId}/schemes/confirm")]
    public async Task<IActionResult> ConfirmPayeScheme(string hashedAccountId, AddNewPayeSchemeViewModel model)
    {
        var result = await _employerAccountPayeOrchestrator.AddPayeSchemeToAccount(model, HttpContext.User.FindFirstValue(ControllerConstants.UserRefClaimKeyName));

        if (result.Status != HttpStatusCode.OK)
        {
            return View(result);
        }

        var payeSchemeName = string.IsNullOrEmpty(model.PayeName) ? "this PAYE scheme" : model.PayeName;

        var flashMessage = new FlashMessageViewModel
        {
            Severity = FlashMessageSeverityLevel.Success,
            Headline = $"{model.PayeScheme} has been added",
            HiddenFlashMessageInformation = "page-paye-scheme-added"
        };
        AddFlashMessageToCookie(flashMessage);

        return RedirectToAction(ControllerConstants.NextStepsActionName, ControllerConstants.EmployerAccountPayeControllerName, new { model.HashedAccountId });
    }

    [HttpGet]
    [Route("{HashedAccountId}/schemes/{empRef}/remove")]
    public async Task<IActionResult> Remove(string hashedAccountId, string empRef)
    {
        var model = await _employerAccountPayeOrchestrator.GetRemovePayeSchemeModel(new RemovePayeSchemeViewModel
        {
            HashedAccountId = hashedAccountId,
            PayeRef = empRef.FormatPayeFromUrl(),
            UserId = HttpContext.User.FindFirstValue(ControllerConstants.UserRefClaimKeyName)
        });

        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [Route("{HashedAccountId}/schemes/remove")]
    public async Task<IActionResult> RemovePaye(string hashedAccountId, RemovePayeSchemeViewModel model)
    {
        model.UserId = HttpContext.User.FindFirstValue(ControllerConstants.UserRefClaimKeyName);

        if (model.RemoveScheme == 1)
        {
            return RedirectToAction(ControllerConstants.IndexActionName, ControllerConstants.EmployerAccountPayeControllerName, new { model.HashedAccountId });
        }

        var result = await _employerAccountPayeOrchestrator.RemoveSchemeFromAccount(model);

        if (result.Status != HttpStatusCode.OK)
        {
            return View(ControllerConstants.RemoveViewName, result);
        }

        model.PayeSchemeName = model.PayeSchemeName ?? string.Empty;

        var flashMessage = new FlashMessageViewModel
        {
            Severity = FlashMessageSeverityLevel.Success,
            Headline = $"You've removed {model.PayeRef}",
            SubMessage = model.PayeSchemeName,
            HiddenFlashMessageInformation = "page-paye-scheme-deleted"
        };

        AddFlashMessageToCookie(flashMessage);

        return RedirectToAction(ControllerConstants.IndexActionName, ControllerConstants.EmployerAccountPayeControllerName, new { model.HashedAccountId });
    }
}