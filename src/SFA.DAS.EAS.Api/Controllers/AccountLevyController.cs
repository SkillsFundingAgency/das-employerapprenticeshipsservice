using System.Web.Http;
using SFA.DAS.EAS.Account.Api.Attributes;
using SFA.DAS.EAS.Domain.Configuration;

namespace SFA.DAS.EAS.Account.Api.Controllers
{
    [RoutePrefix("api/accounts/{hashedAccountId}/levy")]
    public class AccountLevyController : RedirectController
    {
        public AccountLevyController(EmployerApprenticeshipsServiceConfiguration configuration) : base(configuration)
        {
        }

        [Route("", Name = "GetLevy")]
        [ApiAuthorize(Roles = "ReadAllEmployerAccountBalances")]
        [HttpGet]
        public IHttpActionResult Index(string hashedAccountId)
        {
            return RedirectToEmployerFinanceApi();
        }


        [Route("{payrollYear}/{payrollMonth}", Name = "GetLevyForPeriod")]
        [ApiAuthorize(Roles = "ReadAllEmployerAccountBalances")]
        [HttpGet]
        public IHttpActionResult GetLevy(string hashedAccountId, string payrollYear, short payrollMonth)
        {
            return RedirectToEmployerFinanceApi();
        }
    }
}