using System.Security.Claims;
using SFA.DAS.EmployerAccounts.Helpers;

namespace SFA.DAS.EmployerAccounts.Web.ViewComponents;

public class SupportUserBannerViewComponent : ViewComponent
{
    private readonly EmployerTeamOrchestrator _employerTeamOrchestrator;

    public SupportUserBannerViewComponent(EmployerTeamOrchestrator employerTeamOrchestrator)
    {
        _employerTeamOrchestrator = employerTeamOrchestrator;
    }

    public IViewComponentResult Invoke(IAccountIdentifier model = null)
    {
        Account account = null;

        if (model != null && model.HashedAccountId != null)
        {
            var externalUserId = OwinWrapper.GetClaimValue(ControllerConstants.UserRefClaimKeyName);
            var response = AsyncHelper.RunSync(() => _employerTeamOrchestrator.GetAccountSummary(model.HashedAccountId, externalUserId));
            account = response.Status != HttpStatusCode.OK ? null : response.Data.Account;
        }

        var consoleUserType = OwinWrapper.GetClaimValue(ClaimTypes.Role) == "Tier2User" ? "Service user (T2 Support)" : "Standard user";

        return View(new SupportUserBannerViewModel
        {
            Account = account,
            ConsoleUserType = consoleUserType
        });
    }
}