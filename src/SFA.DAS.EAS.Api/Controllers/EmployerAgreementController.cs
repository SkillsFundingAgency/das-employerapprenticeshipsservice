using System.Web.Http;
using SFA.DAS.EAS.Account.Api.Attributes;
using SFA.DAS.EAS.Domain.Configuration;

namespace SFA.DAS.EAS.Account.Api.Controllers
{
    [RoutePrefix("api/accounts/{hashedAccountId}/legalEntities/{hashedlegalEntityId}/agreements")]
    public class EmployerAgreementController : RedirectController
    {
        public EmployerAgreementController(EmployerApprenticeshipsServiceConfiguration configuration) : base(configuration)
        {
        }

        [Route("{agreementId}", Name = "AgreementById")]
        [ApiAuthorize(Roles = "ReadAllEmployerAgreements")]
        [HttpGet]   
        public IHttpActionResult GetAgreement(string agreementId)
        {
            return RedirectToEmployerAccountsApi();
        }
    }
}
