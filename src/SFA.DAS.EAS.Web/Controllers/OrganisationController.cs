using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Configuration;
using SFA.DAS.EAS.Web.Authentication;
using SFA.DAS.EAS.Web.Extensions;

namespace SFA.DAS.EAS.Web.Controllers;

[Authorize(Policy = nameof(PolicyNames.HasUserAccount))]
[Route("accounts/{HashedAccountId}/organisations")]
public class OrganisationController : Controller
{
    public IConfiguration Configuration { get; set; }
    public OrganisationController(IConfiguration _configuration)
    {
        Configuration = _configuration;
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [Route("confirm")]
    public async Task<IActionResult> Confirm()
    {
        return Redirect(Url.EmployerAccountsAction("organisations/confirm", Configuration));
    }

    [HttpGet]
    [Route("nextStep")]
    public async Task<IActionResult> OrganisationAddedNextSteps()
    {
        return Redirect(Url.EmployerAccountsAction($"organisations/nextStep", Configuration));
    }

    [HttpGet]
    [Route("nextStepSearch")]
    public async Task<IActionResult> OrganisationAddedNextStepsSearch()
    {
        return Redirect(Url.EmployerAccountsAction($"organisations/nextStepSearch", Configuration));
    }


    [HttpPost]
    [Route("nextStep")]
    public async Task<IActionResult> GoToNextStep()
    {
        return Redirect(Url.EmployerAccountsAction("nextStep", Configuration));
    }

    [HttpGet]
    [Route("review")]
    public async Task<IActionResult> Review()
    {
        return Redirect(Url.EmployerAccountsAction("organisations/review", Configuration));
    }

    [HttpPost]
    [Route("review")]
    public async Task<IActionResult> ProcessReviewSelection()
    {
        return Redirect(Url.EmployerAccountsAction("organisations/review", Configuration));
    }

    [HttpPost]
    [Route("PostUpdateSelection")]
    public IActionResult GoToPostUpdateSelection()
    {
        return Redirect(Url.EmployerAccountsAction("organisations/PostUpdateSelection", Configuration));
    }
}