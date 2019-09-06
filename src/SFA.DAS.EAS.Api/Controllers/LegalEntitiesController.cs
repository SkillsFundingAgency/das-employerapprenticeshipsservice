using System.Threading.Tasks;
using System.Web.Http;
using SFA.DAS.EAS.Account.Api.Attributes;
using SFA.DAS.EAS.Application.Services.EmployerAccountsApi;

namespace SFA.DAS.EAS.Account.Api.Controllers
{
    [RoutePrefix("api/accounts/{hashedAccountId}/legalentities")]
    public class LegalEntitiesController : ApiController
    {
        private readonly IEmployerAccountsApiService _apiService;

        public LegalEntitiesController(IEmployerAccountsApiService apiService)
        {
            _apiService = apiService;
        }

        [Route("", Name = "GetLegalEntities")]
        [ApiAuthorize(Roles = "ReadAllEmployerAccountBalances")]
        [HttpGet]
        public async Task<IHttpActionResult> GetLegalEntities(string hashedAccountId)
        {
            return Ok(await _apiService.Redirect($"/api/accounts/{hashedAccountId}/legalentities"));
        }

        [Route("{legalEntityId}", Name = "GetLegalEntity")]
        [ApiAuthorize(Roles = "ReadAllEmployerAccountBalances")]
        public async Task<IHttpActionResult> GetLegalEntity(
            string hashedAccountId,
            long legalEntityId)
        {
            return Ok(await _apiService.Redirect($"/api/accounts/{hashedAccountId}/legalentities/{legalEntityId}"));
        }
    }
}