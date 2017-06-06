using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using NLog;
using SFA.DAS.EAS.Domain.Interfaces;
using SFA.DAS.EAS.Web.Authentication;
using SFA.DAS.EAS.Web.Orchestrators;
using SFA.DAS.EAS.Web.ViewModels;
using SFA.DAS.EmployerUsers.WebClientComponents;

namespace SFA.DAS.EAS.Web.Controllers
{
    [RoutePrefix("settings")]
    [AuthoriseActiveUser]
    public class SettingsController : BaseController
    {
        private readonly UserSettingsOrchestrator _userSettingsOrchestrator;

        public SettingsController(IOwinWrapper owinWrapper,
            UserSettingsOrchestrator userSettingsOrchestrator,
            IFeatureToggle featureToggle,
            IMultiVariantTestingService multiVariantTestingService,
            ICookieStorageService<FlashMessageViewModel> flashMessage)
            : base(owinWrapper, featureToggle, multiVariantTestingService, flashMessage)
        {
            _userSettingsOrchestrator = userSettingsOrchestrator;
        }

        [HttpGet]
        [Route("notifications")]
        public async Task<ActionResult> NotificationSettings()
        {
            var userIdClaim = OwinWrapper.GetClaimValue(@"sub");
            var vm = await _userSettingsOrchestrator.GetNotificationSettingsViewModel(userIdClaim);

            var flashMessage = GetFlashMessageViewModelFromCookie();
            if (flashMessage == null)
            {
                flashMessage = new FlashMessageViewModel
                {
                    Severity = FlashMessageSeverityLevel.Info,
                    Message = "Changes to these settings won't affect service emails, such as password resets"
                };
            }

            vm.FlashMessage = flashMessage;

            return View(vm);
        }

        [HttpPost]
        [Route("notifications")]
        public async Task<ActionResult> NotificationSettings(NotificationSettingsViewModel vm)
        {
            var userIdClaim = OwinWrapper.GetClaimValue(@"sub");

            await _userSettingsOrchestrator.UpdateNotificationSettings(userIdClaim,
                vm.NotificationSettings);

            var flashMessage = new FlashMessageViewModel
            {
                Severity = FlashMessageSeverityLevel.Success,
                Message = "Settings updated"
            };

            AddFlashMessageToCookie(flashMessage);

            return RedirectToAction("NotificationSettings");
        }

    }
}