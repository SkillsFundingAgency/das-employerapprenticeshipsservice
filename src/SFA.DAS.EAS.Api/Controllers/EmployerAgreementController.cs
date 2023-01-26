using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SFA.DAS.EAS.Application.Services.EmployerAccountsApi;

namespace SFA.DAS.EAS.Account.Api.Controllers
{
    [ApiController]
    [Route("api/accounts/{hashedAccountId}/legalEntities/{hashedlegalEntityId}/agreements")]
    public class EmployerAgreementController : Microsoft.AspNetCore.Mvc.ControllerBase
    {
        private readonly IEmployerAccountsApiService _apiService;

        public EmployerAgreementController(IEmployerAccountsApiService apiService)
        {
            _apiService = apiService;
        }

        [Authorize(Policy = "LoopBack", Roles = "ReadAllEmployerAgreements")]
        [HttpGet("{hashedAgreementId}", Name = "AgreementById")]
        public async Task<IActionResult> GetAgreement(
            string hashedAccountId,
            string hashedLegalEntityId,
            string hashedAgreementId)
        {
            return Ok(await _apiService.Redirect($"/api/accounts/{hashedAccountId}/legalentities/{hashedLegalEntityId}/agreements/{hashedAgreementId}"));
        }
    }
}
