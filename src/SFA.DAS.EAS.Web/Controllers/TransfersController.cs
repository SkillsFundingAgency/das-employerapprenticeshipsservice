using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using SFA.DAS.Authorization.Mvc.Attributes;
using SFA.DAS.EAS.Web.Extensions;

namespace SFA.DAS.EAS.Web.Controllers;

[DasAuthorize]
[Route("accounts/{HashedAccountId}/transfers")]
public class TransfersController : Microsoft.AspNetCore.Mvc.Controller
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