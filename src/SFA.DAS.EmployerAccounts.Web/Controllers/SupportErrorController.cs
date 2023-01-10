using SFA.DAS.Authorization.Mvc.Attributes;
using SFA.DAS.EmployerAccounts.Helpers;

namespace SFA.DAS.EmployerAccounts.Web.Controllers;

public class SupportErrorController : BaseController
{
    private readonly SupportErrorOrchestrator _orchestrator;

    public SupportErrorController(IAuthenticationService owinWrapper, SupportErrorOrchestrator orchestrator) : base(owinWrapper)
    {
        _orchestrator = orchestrator;
    }

    [DasAuthorize]
    [Route("error/accessdenied/{HashedAccountId}")]
    public IActionResult AccessDenied(string hashedAccountId)
    {
        AccessDeniedViewModel model = new AccessDeniedViewModel { HashedAccountId = hashedAccountId };
        return View(model);
    }

    [ChildActionOnly]
    public override IActionResult SupportUserBanner(IAccountIdentifier model = null)
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