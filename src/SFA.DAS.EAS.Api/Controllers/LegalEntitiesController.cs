using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SFA.DAS.EAS.Account.Api.Authorization;
using SFA.DAS.EAS.Application.Services.EmployerAccountsApi;

namespace SFA.DAS.EAS.Account.Api.Controllers;

[ApiController]
[Route("api/accounts/{hashedAccountId}/legalentities")]
public class LegalEntitiesController : ControllerBase
{
    private readonly IEmployerAccountsApiService _apiService;

    public LegalEntitiesController(IEmployerAccountsApiService apiService)
    {
        _apiService = apiService;
    }

    [Authorize(Roles = ApiRoles.ReadAllEmployerAccountBalances)]
    [HttpGet(Name = "GetLegalEntities")]
    public async Task<IActionResult> GetLegalEntities(string hashedAccountId)
    {
        var redirectResponse = await _apiService.Redirect($"/api/accounts/{hashedAccountId}/legalentities");
        return Content(redirectResponse.ToString(), "application/json");
    }

    [Authorize(Roles = ApiRoles.ReadAllEmployerAccountBalances)]
    [HttpGet("{legalEntityId}", Name = "GetLegalEntity")]
    public async Task<IActionResult> GetLegalEntity(
        string hashedAccountId,
        long legalEntityId)
    {
        var redirectResponse = await _apiService.Redirect($"/api/accounts/{hashedAccountId}/legalentities/{legalEntityId}");
        return Content(redirectResponse.ToString(), "application/json");
    }
}