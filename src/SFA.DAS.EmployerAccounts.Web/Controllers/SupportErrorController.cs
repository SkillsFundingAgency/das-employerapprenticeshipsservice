using System.Net;
using System.Web.Mvc;
using SFA.DAS.Authentication;
using SFA.DAS.Authorization.Mvc.Attributes;
using SFA.DAS.EmployerAccounts.Helpers;
using SFA.DAS.EmployerAccounts.Interfaces;
using SFA.DAS.EmployerAccounts.Web.Helpers;
using SFA.DAS.EmployerAccounts.Web.Orchestrators;
using SFA.DAS.EmployerAccounts.Web.ViewModels;

namespace SFA.DAS.EmployerAccounts.Web.Controllers
{
    public class SupportErrorController : BaseController
    {
        private readonly SupportErrorOrchestrator _orchestrator;

        public SupportErrorController(IAuthenticationService owinWrapper, SupportErrorOrchestrator orchestrator) : base(owinWrapper)
        {
            _orchestrator = orchestrator;
        }

        [DasAuthorize]
        [Route("error/accessdenied/{HashedAccountId}")]
        public Microsoft.AspNetCore.Mvc.ActionResult AccessDenied(string hashedAccountId)
        {
            AccessDeniedViewModel model = new AccessDeniedViewModel { HashedAccountId = hashedAccountId };
            return View(model);
        }

        [ChildActionOnly]
        public override Microsoft.AspNetCore.Mvc.ActionResult SupportUserBanner(IAccountIdentifier model = null)
        {
            EmployerAccounts.Models.Account.Account account = null;

            if (model != null && model.HashedAccountId != null)
            {
                var externalUserId = OwinWrapper.GetClaimValue(ControllerConstants.UserRefClaimKeyName);
                var response = AsyncHelper.RunSync(() => _orchestrator.GetAccountSummary(model.HashedAccountId, externalUserId));

                account = response.Status != HttpStatusCode.OK ? null : response.Data.Account;
            }

            return PartialView("_SupportUserBanner", new SupportUserBannerViewModel() { Account = account });
        }
    }
}