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
        private readonly ILogger _logger;

        public SettingsController(IOwinWrapper owinWrapper,
            UserSettingsOrchestrator userSettingsOrchestrator,
            IFeatureToggle featureToggle,
            IMultiVariantTestingService multiVariantTestingService,
            ILogger logger,
            ICookieStorageService<FlashMessageViewModel> flashMessage)
            : base(owinWrapper, featureToggle, multiVariantTestingService, flashMessage)
        {
            _userSettingsOrchestrator = userSettingsOrchestrator;
            _logger = logger;
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
        public async Task<ActionResult> NotificationSettings(FormCollection collection)
        {
            var userIdClaim = OwinWrapper.GetClaimValue(@"sub");
            var vm = await _userSettingsOrchestrator.GetNotificationSettingsViewModel(userIdClaim);

            //todo: how to do this more cleanly?
            //also, audit updates

            foreach (var key in collection.Keys)
            {
                long k;
                if (long.TryParse(key.ToString(), out k))
                {
                    var setting = vm.Data.NotificationSettings.FirstOrDefault(x => x.AccountId == k);
                    if (setting != null)
                    {
                        setting.ReceiveNotifications = bool.Parse(collection[k.ToString()]);
                    }
                }
            }

            await _userSettingsOrchestrator.UpdateNotificationSettings(userIdClaim,
                vm.Data.NotificationSettings);

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