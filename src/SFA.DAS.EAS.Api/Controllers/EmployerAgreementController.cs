using System.Web.Http;
using SFA.DAS.EAS.Account.Api.Attributes;
using SFA.DAS.EAS.Domain.Configuration;

namespace SFA.DAS.EAS.Account.Api.Controllers
{
    [RoutePrefix("api/accounts/{hashedAccountId}/legalEntities/{hashedlegalEntityId}/agreements")]
    public class EmployerAgreementController : ApiController
    {
        private readonly EmployerAccountsApiConfiguration _configuration;

        public EmployerAgreementController(EmployerAccountsApiConfiguration configuration)
        {
            _configuration = configuration;
        }

        [Route("{hashedAgreementId}", Name = "AgreementById")]
        [ApiAuthorize(Roles = "ReadAllEmployerAgreements")]
        [HttpGet]   
        public IHttpActionResult GetAgreement(
            string hashedAccountId,
            string hashedLegalEntityId,
            string hashedAgreementId)
        {
            return Redirect($"{_configuration.BaseUrl}/api/accounts/{hashedAccountId}/legalEntities/{hashedLegalEntityId}/agreements/{hashedAgreementId}");
        }
    }
}
