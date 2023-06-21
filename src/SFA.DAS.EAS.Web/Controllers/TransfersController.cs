using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Configuration;
using SFA.DAS.EAS.Web.Authentication;
using SFA.DAS.EAS.Web.Extensions;

namespace SFA.DAS.EAS.Web.Controllers;

[Authorize(Policy = nameof(PolicyNames.HasUserAccount))]
[Route("accounts/{HashedAccountId}/transfers")]
public class TransfersController : Controller
{
    public IConfiguration Configuration { get; set; }
    public TransfersController(IConfiguration _configuration)
    {
        Configuration = _configuration;
    }

    public IActionResult Index()
    {
        return Redirect(Url.EmployerFinanceAction("transfers/connections", Configuration));
    }
}