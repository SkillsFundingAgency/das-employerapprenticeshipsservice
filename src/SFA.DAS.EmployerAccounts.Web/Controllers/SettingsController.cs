using System.Security.Claims;
using SFA.DAS.Authorization.Mvc.Attributes;
using SFA.DAS.Encoding;

namespace SFA.DAS.EmployerAccounts.Web.Controllers;

[Route("settings")]
[DasAuthorize]
public class SettingsController : BaseController
{
    private readonly UserSettingsOrchestrator _userSettingsOrchestrator;
    private readonly IEncodingService _encodingService;

    public SettingsController(
        UserSettingsOrchestrator userSettingsOrchestrator,
        ICookieStorageService<FlashMessageViewModel> flashMessage,
        IMultiVariantTestingService multiVariantTestingService,
        IEncodingService encodingService)
        : base(flashMessage, multiVariantTestingService)
    {
        _userSettingsOrchestrator = userSettingsOrchestrator;
        _encodingService = encodingService;
    }

    [HttpGet]
    [Route("notifications")]
    public async Task<IActionResult> NotificationSettings()
    {
        var userIdClaim = HttpContext.User.FindFirstValue(ControllerConstants.UserRefClaimKeyName);
        var vm = await _userSettingsOrchestrator.GetNotificationSettingsViewModel(userIdClaim);

        var flashMessage = GetFlashMessageViewModelFromCookie();

        vm.FlashMessage = flashMessage;

        return View(vm);
    }

    [HttpPost]
    [Route("notifications")]
    public async Task<IActionResult> NotificationSettings(NotificationSettingsViewModel vm)
    {
        var userIdClaim = HttpContext.User.FindFirstValue(ControllerConstants.UserRefClaimKeyName);

        var accountId = _encodingService.Decode(vm.HashedId, EncodingType.AccountId);

        await _userSettingsOrchestrator.UpdateNotificationSettings(accountId, userIdClaim, vm.NotificationSettings);

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
        var userIdClaim = HttpContext.User.FindFirstValue(ControllerConstants.UserRefClaimKeyName);
        
        var accountId = _encodingService.Decode(hashedAccountId, EncodingType.AccountId);

        var model = await _userSettingsOrchestrator.Unsubscribe(userIdClaim, accountId);

        return View(model);
    }
}