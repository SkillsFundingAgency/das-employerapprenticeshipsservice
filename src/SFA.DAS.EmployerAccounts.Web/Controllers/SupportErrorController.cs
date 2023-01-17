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
}