using System.Threading.Tasks;
using System.Web.Http;
using SFA.DAS.EAS.Account.Api.Attributes;
using SFA.DAS.EAS.Application.Queries.GetLegalEntity;
using SFA.DAS.EAS.Domain.Configuration;

namespace SFA.DAS.EAS.Account.Api.Controllers
{
    [RoutePrefix("api/accounts/{hashedAccountId}/legalentities")]
    public class LegalEntitiesController : ApiController
    {
        private readonly EmployerApprenticeshipsServiceConfiguration _configuration;

        public LegalEntitiesController(EmployerApprenticeshipsServiceConfiguration configuration)
        {
            _configuration = configuration;
        }

        [Route("", Name = "GetLegalEntities")]
        [ApiAuthorize(Roles = "ReadAllEmployerAccountBalances")]
        [HttpGet]
        public async Task<IHttpActionResult> GetLegalEntities(string hashedAccountId)
        {
            return Redirect(_configuration.EmployerAccountsApiBaseUrl + $"/api/accounts/{hashedAccountId}/legalentities");
        }

        [Route("{legalEntityId}", Name = "GetLegalEntity")]
        [ApiAuthorize(Roles = "ReadAllEmployerAccountBalances")]
        public async Task<IHttpActionResult> GetLegalEntity([FromUri] GetLegalEntityQuery query)
        {
            return Redirect(_configuration.EmployerAccountsApiBaseUrl + $"/api/accounts/{query.AccountHashedId}/legalentities/{query.LegalEntityId}");
        }
    }
}