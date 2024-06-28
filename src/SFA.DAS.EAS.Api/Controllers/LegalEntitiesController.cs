using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SFA.DAS.EAS.Account.Api.Authorization;
using SFA.DAS.EAS.Application.Services.EmployerAccountsApi;
using SFA.DAS.Encoding;

namespace SFA.DAS.EAS.Account.Api.Controllers;

[ApiController]
[Route("api/accounts/{hashedAccountId}/legalentities")]
public class LegalEntitiesController : ControllerBase
{
    private readonly IEmployerAccountsApiService _apiService;
    private readonly IEncodingService _encodingService;

    public LegalEntitiesController(IEmployerAccountsApiService apiService, IEncodingService encodingService)
    {
        _apiService = apiService;
        _encodingService = encodingService;
    }

    [Authorize(Policy = ApiRoles.ReadAllEmployerAccountBalances)]
    [HttpGet(Name = "GetLegalEntities")]
    public async Task<IActionResult> GetLegalEntities(string hashedAccountId)
    {
        var accountId = _encodingService.Decode(hashedAccountId, EncodingType.AccountId);
        var redirectResponse = await _apiService.Redirect($"/api/accounts/{accountId}/legalentities");
        return Content(redirectResponse.ToString(), "application/json");
    }

    [Authorize(Policy = ApiRoles.ReadAllEmployerAccountBalances)]
    [HttpGet("{legalEntityId}", Name = "GetLegalEntity")]
    public async Task<IActionResult> GetLegalEntity(
        string hashedAccountId,
        long legalEntityId)
    {
        var accountId = _encodingService.Decode(hashedAccountId, EncodingType.AccountId);
        var redirectResponse = await _apiService.Redirect($"/api/accounts/{accountId}/legalentities/{legalEntityId}");
        return Content(redirectResponse.ToString(), "application/json");
    }
}