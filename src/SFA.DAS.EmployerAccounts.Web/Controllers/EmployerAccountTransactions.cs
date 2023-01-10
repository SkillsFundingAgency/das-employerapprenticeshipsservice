using SFA.DAS.Authorization.Mvc.Attributes;

namespace SFA.DAS.EmployerAccounts.Web.Controllers;

[DasAuthorize]
[Route("accounts/{HashedAccountId}")]
public class EmployerAccountTransactionsController : Controller
{
    private readonly IUrlActionHelper _urlHelper;

    public EmployerAccountTransactionsController(IUrlActionHelper urlHelper)
    {
        _urlHelper = urlHelper;
    }

    [Route("finance")]
    [Route("balance")]
    public IActionResult Index(string hashedAccountId)
    {
        return Redirect(_urlHelper.EmployerFinanceAction(RouteData,"finance"));
    }
}