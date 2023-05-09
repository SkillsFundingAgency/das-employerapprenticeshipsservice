using Microsoft.Extensions.Configuration;
using SFA.DAS.EAS.Web.Extensions;
using SFA.DAS.EmployerUsers.WebClientComponents;

namespace SFA.DAS.EAS.Web.Controllers;

[Route("accounts")]
[AuthoriseActiveUser]
public class EmployerAccountController : Controller
{
    public IConfiguration Configuration { get; set; }

    public EmployerAccountController(IConfiguration _configuration)
    {
        Configuration = _configuration;
    }

    [HttpGet]
    [Route("gatewayInform")]
    public IActionResult GatewayInform()
    {
        return Redirect(Url.EmployerAccountsAction("gatewayInform", Configuration));
    }

    [HttpGet]
    [Route("gateway")]
    public IActionResult Gateway()
    {
        return Redirect(Url.EmployerAccountsAction("gateway", Configuration));
    }

    [Route("gatewayResponse")]
    public IActionResult GateWayResponse()
    {
        return Redirect(Url.EmployerAccountsAction($"gatewayResponse{Request.QueryString}", Configuration));
    }

    [HttpGet]
    [Route("summary")]
    public IActionResult Summary()
    {
        return Redirect(Url.EmployerAccountsAction("summary", Configuration));
    }

    [HttpGet]
    [Route("create")]
    public IActionResult Create()
    {
        return Redirect(Url.EmployerAccountsAction("create", Configuration));
    }

    [HttpGet]
    [Route("{HashedAccountId}/rename")]
    public IActionResult RenameAccount(string hashedAccountId)
    {
        return Redirect(Url.EmployerAccountsAction("rename", Configuration));
    }
}