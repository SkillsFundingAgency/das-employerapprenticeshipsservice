using System.Web.Http;
using SFA.DAS.EAS.Account.Api.Attributes;
using SFA.DAS.EAS.Domain.Configuration;

namespace SFA.DAS.EAS.Account.Api.Controllers
{
    [RoutePrefix("api/accounts/{hashedAccountId}/legalentities")]
    public class LegalEntitiesController : ApiController
    {
        private readonly EmployerAccountsApiConfiguration _configuration;

        public LegalEntitiesController(EmployerAccountsApiConfiguration configuration)
        {
            _configuration = configuration;
        }

        [Route("", Name = "GetLegalEntities")]
        [ApiAuthorize(Roles = "ReadAllEmployerAccountBalances")]
        [HttpGet]
        public IHttpActionResult GetLegalEntities(string hashedAccountId)
        {
            return Redirect(_configuration.BaseUrl + $"/api/accounts/{hashedAccountId}/legalentities");
        }

        [Route("{legalEntityId}", Name = "GetLegalEntity")]
        [ApiAuthorize(Roles = "ReadAllEmployerAccountBalances")]
        public IHttpActionResult GetLegalEntity(
            string hashedAccountId,
            long legalEntityId)
        {
            return Redirect(_configuration.BaseUrl + $"/api/accounts/{hashedAccountId}/legalentities/{legalEntityId}");
        }
    }
}