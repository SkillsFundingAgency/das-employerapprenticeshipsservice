using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SFA.DAS.EAS.Account.Api.Authorization;
using SFA.DAS.EAS.Application.Services.EmployerAccountsApi;

namespace SFA.DAS.EAS.Account.Api.Controllers;

[ApiController]
[Route("api/accounts/{hashedAccountId}/legalEntities/{hashedlegalEntityId}/agreements")]
public class EmployerAgreementController : ControllerBase
{
    private readonly IEmployerAccountsApiService _apiService;

    public EmployerAgreementController(IEmployerAccountsApiService apiService)
    {
        _apiService = apiService;
    }

    [Authorize(Roles = ApiRoles.ReadAllEmployerAgreements)]
    [HttpGet("{hashedAgreementId}", Name = "AgreementById")]
    public async Task<IActionResult> GetAgreement(
        string hashedAccountId,
        string hashedLegalEntityId,
        string hashedAgreementId)
    {
        return Ok(await _apiService.Redirect($"/api/accounts/{hashedAccountId}/legalentities/{hashedLegalEntityId}/agreements/{hashedAgreementId}"));
    }
}
