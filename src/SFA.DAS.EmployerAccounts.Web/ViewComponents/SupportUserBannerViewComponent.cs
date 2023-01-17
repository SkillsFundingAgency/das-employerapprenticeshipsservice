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

    public async Task<IViewComponentResult> InvokeAsync(IAccountIdentifier model = null)
    {
        Account account = null;

        if (model != null && model.HashedAccountId != null)
        {
            var externalUserId = OwinWrapper.GetClaimValue(ControllerConstants.UserRefClaimKeyName);
            var response = await _employerTeamOrchestrator.GetAccountSummary(model.HashedAccountId, externalUserId);
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