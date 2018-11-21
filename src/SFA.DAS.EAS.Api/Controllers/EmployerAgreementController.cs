using System.Web.Http;
using SFA.DAS.EAS.Account.Api.Attributes;
using SFA.DAS.EAS.Account.Api.Extensions;

namespace SFA.DAS.EAS.Account.Api.Controllers
{
    [RoutePrefix("api/accounts/{hashedAccountId}/legalEntities/{hashedlegalEntityId}/agreements")]
    public class EmployerAgreementController : ApiController
    {
        [Route("{agreementId}", Name = "AgreementById")]
        [ApiAuthorize(Roles = "ReadAllEmployerAgreements")]
        [HttpGet]   
        public IHttpActionResult GetAgreement(string agreementId)
        {
            return Redirect(Url.EmployerAccountsApiAction(Request.RequestUri.PathAndQuery));
        }
    }
}
