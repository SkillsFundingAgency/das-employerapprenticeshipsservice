using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Configuration;
using SFA.DAS.EAS.Web.Authentication;
using SFA.DAS.EAS.Web.Extensions;

namespace SFA.DAS.EAS.Web.Controllers;

[Authorize(Policy = nameof(PolicyNames.HasUserAccount))]
[Route("accounts")]
public class SearchOrganisationController : Controller
{
    private readonly IConfiguration _configuration;
    
    public SearchOrganisationController(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    [HttpGet]
    [Route("{HashedAccountId}/organisations/search", Order = 0)]
    [Route("organisations/search", Order = 1)]
    public IActionResult SearchForOrganisation()
    {
        return Redirect(Url.EmployerAccountsAction("organisations/search", _configuration));
    }
    
    [Route("{HashedAccountId}/organisations/search/results", Order = 0)]
    [Route("organisations/search/results", Order = 1)]
    public IActionResult SearchForOrganisationResults()
    {
        var paramString = Request?.Query == null ? string.Empty : $"?{Request.Query}";

        return Redirect(Url.EmployerAccountsAction($"organisations/search/results{paramString}", _configuration));
    }
}