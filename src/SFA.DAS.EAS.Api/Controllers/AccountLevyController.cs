using System.Web.Http;
using SFA.DAS.EAS.Account.Api.Attributes;
using SFA.DAS.EAS.Account.Api.Extensions;

namespace SFA.DAS.EAS.Account.Api.Controllers
{
    [RoutePrefix("api/accounts/{hashedAccountId}/levy")]
    public class AccountLevyController : ApiController
    {
        [Route("", Name = "GetLevy")]
        [ApiAuthorize(Roles = "ReadAllEmployerAccountBalances")]
        [HttpGet]
        public IHttpActionResult Index(string hashedAccountId)
        {
            return Redirect(Url.EmployerFinanceApiAction(Request.RequestUri.PathAndQuery));
        }

        [Route("{payrollYear}/{payrollMonth}", Name = "GetLevyForPeriod")]
        [ApiAuthorize(Roles = "ReadAllEmployerAccountBalances")]
        [HttpGet]
        public IHttpActionResult GetLevy(string hashedAccountId, string payrollYear, short payrollMonth)
        {
            return Redirect(Url.EmployerFinanceApiAction(Request.RequestUri.PathAndQuery));
        }
    }
}