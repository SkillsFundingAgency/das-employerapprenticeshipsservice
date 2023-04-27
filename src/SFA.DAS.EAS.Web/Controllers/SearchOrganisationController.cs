﻿using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using SFA.DAS.Authorization.Mvc.Attributes;
using SFA.DAS.EAS.Web.Extensions;

namespace SFA.DAS.EAS.Web.Controllers;

[DasAuthorize]
[Route("accounts")]
public class SearchOrganisationController : Controller
{
    public IConfiguration Configuration { get; set; }
    public SearchOrganisationController(IConfiguration _configuration)
    {
        Configuration = _configuration;
    }

    [HttpGet]
    [Route("{HashedAccountId}/organisations/search", Order = 0)]
    [Route("organisations/search", Order = 1)]
    public IActionResult SearchForOrganisation()
    {
        return Redirect(Url.EmployerAccountsAction("organisations/search", Configuration));
    }
    
    [Route("{HashedAccountId}/organisations/search/results", Order = 0)]
    [Route("organisations/search/results", Order = 1)]
    public async Task<IActionResult> SearchForOrganisationResults()
    {
        var paramString = Request?.Query == null ? string.Empty : $"?{Request.Query}";

        return Redirect(Url.EmployerAccountsAction($"organisations/search/results{paramString}", Configuration));
    }
}