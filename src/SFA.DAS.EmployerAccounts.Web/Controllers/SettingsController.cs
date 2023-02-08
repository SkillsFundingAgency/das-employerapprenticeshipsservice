using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Web.Mvc;
using SFA.DAS.Authentication;
using SFA.DAS.Authorization.Mvc.Attributes;
using SFA.DAS.EmployerAccounts.Interfaces;
using SFA.DAS.EmployerAccounts.Web.Helpers;
using SFA.DAS.EmployerAccounts.Web.Orchestrators;
using SFA.DAS.EmployerAccounts.Web.ViewModels;

namespace SFA.DAS.EmployerAccounts.Web.Controllers
{
    [RoutePrefix("settings")]
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
        public async Task<ActionResult> NotificationSettings()
        {
            var userIdClaim = OwinWrapper.GetClaimValue(ControllerConstants.UserRefClaimKeyName);
            var vm = await _userSettingsOrchestrator.GetNotificationSettingsViewModel(userIdClaim);

            var flashMessage = GetFlashMessageViewModelFromCookie();

            vm.FlashMessage = flashMessage;

            return View(vm);
        }

        [HttpPost]
        [Route("notifications")]
        public async Task<ActionResult> NotificationSettings(NotificationSettingsViewModel vm)
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
        public async Task<ActionResult> NotificationUnsubscribe(string hashedAccountId)
        {
            var userIdClaim = OwinWrapper.GetClaimValue(ControllerConstants.UserRefClaimKeyName);

            var url = Url.Action(ControllerConstants.NotificationSettingsActionName);
            var model = await _userSettingsOrchestrator.Unsubscribe(userIdClaim, hashedAccountId, url);

            return View(model);
        }

        [HttpGet]
        [Route("change-signin-details")]
        public ActionResult ChangeSignInDetails()
        {
            return View();
        }
    }
}