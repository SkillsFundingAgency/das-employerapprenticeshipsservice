using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SFA.DAS.EAS.Application.Services.EmployerAccountsApi;

namespace SFA.DAS.EAS.Account.Api.Controllers
{
    [ApiController]
    [Route("api/accounts/{hashedAccountId}/legalentities")]
    public class LegalEntitiesController : ControllerBase
    {
        private readonly IEmployerAccountsApiService _apiService;

        public LegalEntitiesController(IEmployerAccountsApiService apiService)
        {
            _apiService = apiService;
        }

        [Authorize(Policy = "LoopBack", Roles = "ReadAllEmployerAccountBalances")]
        [HttpGet(Name = "GetLegalEntities")]
        public async Task<IActionResult> GetLegalEntities(string hashedAccountId)
        {
            return Ok(await _apiService.Redirect($"/api/accounts/{hashedAccountId}/legalentities"));
        }

        [Authorize(Policy = "LoopBack", Roles = "ReadAllEmployerAccountBalances")]
        [HttpGet("{legalEntityId}", Name = "GetLegalEntity")]
        public async Task<IActionResult> GetLegalEntity(
            string hashedAccountId,
            long legalEntityId)
        {
            return Ok(await _apiService.Redirect($"/api/accounts/{hashedAccountId}/legalentities/{legalEntityId}"));
        }
    }
}