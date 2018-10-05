using SFA.DAS.Authentication;
using SFA.DAS.Authorization;
using SFA.DAS.EmployerAccounts.Interfaces;
using SFA.DAS.EmployerAccounts.Web.Extensions;
using SFA.DAS.EmployerAccounts.Web.Helpers;
using SFA.DAS.EmployerAccounts.Web.Orchestrators;
using SFA.DAS.EmployerAccounts.Web.ViewModels;
using SFA.DAS.EmployerUsers.WebClientComponents;
using SFA.DAS.NLog.Logger;
using System.Net;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace SFA.DAS.EmployerAccounts.Web.Controllers
{
    [RoutePrefix("accounts")]
    [AuthoriseActiveUser]
    public class EmployerAccountController : BaseController
    {
        private readonly EmployerAccountOrchestrator _employerAccountOrchestrator;
        private readonly ILog _logger;

        public EmployerAccountController(IAuthenticationService owinWrapper, EmployerAccountOrchestrator employerAccountOrchestrator,
            IAuthorizationService authorization, IMultiVariantTestingService multiVariantTestingService, ILog logger,
            ICookieStorageService<FlashMessageViewModel> flashMessage)
            : base(owinWrapper, multiVariantTestingService, flashMessage)
        {
            _employerAccountOrchestrator = employerAccountOrchestrator;
            _logger = logger;
        }

        [HttpGet]
        [Route("gatewayInform")]
        public ActionResult GatewayInform()
        {
            return Redirect(Url.LegacyEasAccountAction("gatewayInform"));
        }

        [HttpGet]
        [Route("gateway")]
        public ActionResult Gateway()
        {
            return Redirect(Url.LegacyEasAccountAction("gateway"));
        }

        [Route("gatewayResponse")]
        public ActionResult GateWayResponse()
        {
            return Redirect(Url.LegacyEasAccountAction("gatewayResponse"));
        }

        [HttpGet]
        [Route("summary")]
        public ActionResult Summary()
        {
            return Redirect(Url.LegacyEasAccountAction("summary"));
        }

        [HttpGet]
        [Route("create")]
        public ActionResult Create()
        {
            return Redirect(Url.LegacyEasAccountAction("create"));
        }

        [HttpGet]
        [Route("{HashedAccountId}/rename")]
        public async Task<ActionResult> RenameAccount(string hashedAccountId)
        {
            var userIdClaim = OwinWrapper.GetClaimValue(ControllerConstants.UserRefClaimKeyName);
            var vm = await _employerAccountOrchestrator.GetRenameEmployerAccountViewModel(hashedAccountId, userIdClaim);
            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("{HashedAccountId}/rename")]
        public async Task<ActionResult> RenameAccount(RenameEmployerAccountViewModel vm)
        {
            var userIdClaim = OwinWrapper.GetClaimValue(ControllerConstants.UserRefClaimKeyName);
            var response = await _employerAccountOrchestrator.RenameEmployerAccount(vm, userIdClaim);

            if (response.Status == HttpStatusCode.OK)
            {
                var flashmessage = new FlashMessageViewModel
                {
                    Headline = "Account renamed",
                    Message = "You successfully updated the account name",
                    Severity = FlashMessageSeverityLevel.Success
                };

                AddFlashMessageToCookie(flashmessage);

                return RedirectToAction(ControllerConstants.IndexActionName, ControllerConstants.EmployerTeamActionName);
            }

            var errorResponse = new OrchestratorResponse<RenameEmployerAccountViewModel>();

            if (response.Status == HttpStatusCode.BadRequest)
            {
                vm.ErrorDictionary = response.FlashMessage.ErrorMessages;
            }

            errorResponse.Data = vm;
            errorResponse.FlashMessage = response.FlashMessage;
            errorResponse.Status = response.Status;

            return View(errorResponse);
        }
    }
}