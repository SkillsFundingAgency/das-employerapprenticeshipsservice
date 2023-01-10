using SFA.DAS.Authorization.Mvc.Attributes;

namespace SFA.DAS.EmployerAccounts.Web.Controllers;

[Route("settings")]
[DasAuthorize]
public class SettingsController : BaseController
{
    private readonly UserSettingsOrchestrator _userSettingsOrchestrator;

    public SettingsController(IAuthenticationService owinWrapper,
        UserSettingsOrchestrator userSettingsOrchestrator,
        IMultiVariantTestingService multiVariantTestingService,
        ICookieStorageService<FlashMessageViewModel> flashMessage)
        : base(owinWrapper, multiVariantTestingService, flashMessage)
    {
        _userSettingsOrchestrator = userSettingsOrchestrator;
    }

    [HttpGet]
    [Route("notifications")]
    public async Task<IActionResult> NotificationSettings()
    {
        var userIdClaim = OwinWrapper.GetClaimValue(ControllerConstants.UserRefClaimKeyName);
        var vm = await _userSettingsOrchestrator.GetNotificationSettingsViewModel(userIdClaim);

        var flashMessage = GetFlashMessageViewModelFromCookie();

        vm.FlashMessage = flashMessage;

        return View(vm);
    }

    [HttpPost]
    [Route("notifications")]
    public async Task<IActionResult> NotificationSettings(NotificationSettingsViewModel vm)
    {
        var userIdClaim = OwinWrapper.GetClaimValue(ControllerConstants.UserRefClaimKeyName);

        await _userSettingsOrchestrator.UpdateNotificationSettings(userIdClaim,
            vm.NotificationSettings);

        var flashMessage = new FlashMessageViewModel
        {
            Severity = FlashMessageSeverityLevel.Success,
            Message = "Settings updated."
        };

        AddFlashMessageToCookie(flashMessage);

        return RedirectToAction(ControllerConstants.NotificationSettingsActionName);
    }

    [HttpGet]
    [Route("notifications/unsubscribe/{hashedAccountId}")]
    public async Task<IActionResult> NotificationUnsubscribe(string hashedAccountId)
    {
        var userIdClaim = OwinWrapper.GetClaimValue(ControllerConstants.UserRefClaimKeyName);

        var url = Url.Action(ControllerConstants.NotificationSettingsActionName);
        var model = await _userSettingsOrchestrator.Unsubscribe(userIdClaim, hashedAccountId, url);

        return View(model);
    }
}