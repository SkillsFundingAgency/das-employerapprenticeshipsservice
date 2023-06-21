using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Configuration;
using SFA.DAS.EAS.Web.Authentication;
using SFA.DAS.EAS.Web.Extensions;

namespace SFA.DAS.EAS.Web.Controllers;

[Authorize(Policy = nameof(PolicyNames.HasUserAccount))]
[Route("accounts/{HashedAccountId}/teams")]
public class EmployerTeamController : Controller
{
    public IConfiguration Configuration { get; set; }
    public EmployerTeamController(IConfiguration _configuration)
    {
        Configuration = _configuration;
    }
    [HttpGet]
    [Route("")]
    public IActionResult Index(string hashedAccountId)
    {
        return Redirect(Url.EmployerAccountsAction($"teams?{Request.QueryString}", Configuration));
    }

    [HttpGet]
    [Route("view")]
    public IActionResult ViewTeam(string hashedAccountId)
    {
        return Redirect(Url.EmployerAccountsAction($"teams/view?{Request.QueryString}", Configuration));
    }

    [HttpGet]
    [Route("invite")]
    public IActionResult Invite(string hashedAccountId)
    {
        return Redirect(Url.EmployerAccountsAction($"teams/invite?{Request.QueryString}", Configuration));
    }

    [HttpGet]
    [Route("invite/next")]
    public IActionResult NextSteps(string hashedAccountId)
    {
        return Redirect(Url.EmployerAccountsAction($"teams/invite/next?{Request.QueryString}", Configuration));
    }


    [HttpGet]
    [Route("{invitationId}/cancel")]
    public IActionResult Cancel(string email, string invitationId, string hashedAccountId)
    {
        return Redirect(Url.EmployerAccountsAction($"teams/{invitationId}/cancel?{Request.QueryString}", Configuration));
    }

    [HttpGet]
    [Route("{email}/remove")]
    public IActionResult Remove(string hashedAccountId, string email)
    {
        return Redirect(Url.EmployerAccountsAction($"teams/{email}/remove?{Request.QueryString}", Configuration));
    }

    [HttpGet]
    [Route("{email}/role/change")]
    public IActionResult ChangeRole(string hashedAccountId, string email)
    {
        return Redirect(Url.EmployerAccountsAction($"teams/{email}/role/change?{Request.QueryString}", Configuration));
    }


    [HttpGet]
    [Route("{email}/review")]
    public IActionResult Review(string hashedAccountId, string email)
    {
        return Redirect(Url.EmployerAccountsAction($"teams/{email}/review?{Request.QueryString}", Configuration));
    }

    [HttpGet]
    [Route("hideWizard")]
    public IActionResult HideWizard(string hashedAccountId)
    {
        return Redirect(Url.EmployerAccountsAction($"teams/hideWizard?{Request.QueryString}", Configuration));
    }
}