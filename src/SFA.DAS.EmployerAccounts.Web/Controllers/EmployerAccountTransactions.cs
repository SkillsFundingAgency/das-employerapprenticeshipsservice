using Microsoft.AspNetCore.Authorization;
using SFA.DAS.EmployerAccounts.Web.Authentication;

namespace SFA.DAS.EmployerAccounts.Web.Controllers;

[Authorize(Policy = nameof(PolicyNames.HasEmployerViewerTransactorOwnerAccount))]
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
        return Redirect(_urlHelper.EmployerFinanceAction("finance"));
    }
}