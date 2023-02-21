using Microsoft.AspNetCore.Authorization;
using SFA.DAS.EmployerAccounts.Web.Authentication;

namespace SFA.DAS.EmployerAccounts.Web.Controllers;

public class SupportErrorController : Controller
{
    [Authorize(Policy = nameof(PolicyNames.HasEmployerViewerTransactorOwnerAccount))]
    [Route("error/accessdenied/{HashedAccountId}")]
    public IActionResult AccessDenied(string hashedAccountId)
    {
        var model = new AccessDeniedViewModel { HashedAccountId = hashedAccountId };
        return View(model);
    }
}