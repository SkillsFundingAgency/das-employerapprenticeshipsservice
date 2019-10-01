using System.Threading.Tasks;
using System.Web.Http;
using SFA.DAS.EAS.Account.Api.Attributes;
using SFA.DAS.EAS.Application.Services.EmployerAccountsApi;

namespace SFA.DAS.EAS.Account.Api.Controllers
{
    [RoutePrefix("api/accounts/{hashedAccountId}/legalEntities/{hashedlegalEntityId}/agreements")]
    public class EmployerAgreementController : ApiController
    {
        private readonly IEmployerAccountsApiService _apiService;

        public EmployerAgreementController(IEmployerAccountsApiService apiService)
        {
            _apiService = apiService;
        }

        [Route("{hashedAgreementId}", Name = "AgreementById")]
        [ApiAuthorize(Roles = "ReadAllEmployerAgreements")]
        [HttpGet]
        public async Task<IHttpActionResult> GetAgreement(
            string hashedAccountId,
            string hashedLegalEntityId,
            string hashedAgreementId)
        {
            return Ok(await _apiService.Redirect($"/api/accounts/{hashedAccountId}/legalentities/{hashedLegalEntityId}/agreements/{hashedAgreementId}"));
        }
    }
}
