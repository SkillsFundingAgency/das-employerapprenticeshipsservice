using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Configuration;
using SFA.DAS.EAS.Web.Authentication;
using SFA.DAS.EAS.Web.Extensions;

namespace SFA.DAS.EAS.Web.Controllers;

[Authorize(Policy = nameof(PolicyNames.HasUserAccount))]
[Route("accounts/{HashedAccountId}")]
public class EmployerAccountPayeController : Controller
{
    public IConfiguration Configuration { get; set; }
    public EmployerAccountPayeController(IConfiguration _configuration)
    {
        Configuration = _configuration;
    }

    [HttpGet]
    [Route("schemes")]
    public IActionResult Index(string hashedAccountId)
    {
        return Redirect(Url.EmployerAccountsAction("schemes", Configuration));
    }

    [HttpGet]
    [Route("schemes/next")]
    public IActionResult NextSteps(string hashedAccountId)
    {
        return Redirect(Url.EmployerAccountsAction("schemes/next", Configuration));
    }

    [HttpGet]
    [Route("schemes/{empRef}/details")]
    public IActionResult Details(string hashedAccountId, string empRef)
    {
        return Redirect(Url.EmployerAccountsAction($"schemes/{empRef}/details", Configuration));
    }

    [HttpGet]
    [Route("schemes/gatewayInform")]
    public IActionResult GatewayInform(string hashedAccountId)
    {
        return Redirect(Url.EmployerAccountsAction("schemes/gatewayInform", Configuration));
    }
    
    [HttpGet]
    [Route("schemes/gateway")]
    public IActionResult GetGateway(string hashedAccountId)
    {
        return Redirect(Url.EmployerAccountsAction("schemes/gateway", Configuration));
    }

    [HttpGet]
    [Route("schemes/confirm")]
    public IActionResult ConfirmPayeScheme(string hashedAccountId)
    {
        return Redirect(Url.EmployerAccountsAction($"/schemes/confirm{Request.QueryString}", Configuration));
    }
    
    [HttpGet]
    [Route("schemes/{empRef}/remove")]
    public IActionResult Remove(string hashedAccountId, string empRef)
    {
        return Redirect(Url.EmployerAccountsAction($"schemes/{empRef}/remove", Configuration));
    }
}