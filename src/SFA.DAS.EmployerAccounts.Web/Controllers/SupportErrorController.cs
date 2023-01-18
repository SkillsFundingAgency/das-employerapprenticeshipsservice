using SFA.DAS.Authorization.Mvc.Attributes;

namespace SFA.DAS.EmployerAccounts.Web.Controllers;

public class SupportErrorController : Controller
{
    [DasAuthorize]
    [Route("error/accessdenied/{HashedAccountId}")]
    public IActionResult AccessDenied(string hashedAccountId)
    {
        var model = new AccessDeniedViewModel { HashedAccountId = hashedAccountId };
        return View(model);
    }
}