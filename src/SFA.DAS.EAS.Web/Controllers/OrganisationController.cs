using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Configuration;
using SFA.DAS.EAS.Web.Authentication;
using SFA.DAS.EAS.Web.Extensions;

namespace SFA.DAS.EAS.Web.Controllers;

[Authorize(Policy = nameof(PolicyNames.HasUserAccount))]
[Route("accounts/{HashedAccountId}/organisations")]
public class OrganisationController : Controller
{
    private readonly IConfiguration _configuration;
    
    public OrganisationController(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [Route("confirm")]
    public IActionResult Confirm()
    {
        return Redirect(Url.EmployerAccountsAction("organisations/confirm", _configuration));
    }

    [HttpGet]
    [Route("nextStep")]
    public IActionResult OrganisationAddedNextSteps()
    {
        return Redirect(Url.EmployerAccountsAction($"organisations/nextStep", _configuration));
    }

    [HttpGet]
    [Route("nextStepSearch")]
    public IActionResult OrganisationAddedNextStepsSearch()
    {
        return Redirect(Url.EmployerAccountsAction($"organisations/nextStepSearch", _configuration));
    }


    [HttpPost]
    [Route("nextStep")]
    public IActionResult GoToNextStep()
    {
        return Redirect(Url.EmployerAccountsAction("nextStep", _configuration));
    }

    [HttpGet]
    [Route("review")]
    public IActionResult Review()
    {
        return Redirect(Url.EmployerAccountsAction("organisations/review", _configuration));
    }

    [HttpPost]
    [Route("review")]
    public IActionResult ProcessReviewSelection()
    {
        return Redirect(Url.EmployerAccountsAction("organisations/review", _configuration));
    }

    [HttpPost]
    [Route("PostUpdateSelection")]
    public IActionResult GoToPostUpdateSelection()
    {
        return Redirect(Url.EmployerAccountsAction("organisations/PostUpdateSelection", _configuration));
    }
}