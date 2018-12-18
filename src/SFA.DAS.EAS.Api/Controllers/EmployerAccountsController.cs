using System.Web.Http;
using SFA.DAS.EAS.Account.Api.Attributes;
using SFA.DAS.EAS.Domain.Configuration;

namespace SFA.DAS.EAS.Account.Api.Controllers
{
    [RoutePrefix("api/accounts")]
    public class EmployerAccountsController : RedirectController
    {
        public EmployerAccountsController(EmployerApprenticeshipsServiceConfiguration configuration) : base(configuration)
        {
        }

        [Route("", Name = "AccountsIndex")]
        [ApiAuthorize(Roles = "ReadAllEmployerAccountBalances")]
        [HttpGet]   
        public IHttpActionResult GetAccounts(string toDate = null, int pageSize = 1000, int pageNumber = 1)
        {
            return RedirectToEmployerAccountsApi();
        }

        [Route("{hashedAccountId}", Name = "GetAccount")]
        [ApiAuthorize(Roles = "ReadAllEmployerAccountBalances")]
        [HttpGet]
        public IHttpActionResult GetAccount(string hashedAccountId)
        {
            return RedirectToEmployerAccountsApi();
        }

        [Route("internal/{accountId}", Name = "GetAccountByInternalId")]
        [ApiAuthorize(Roles = "ReadAllEmployerAccountBalances")]
        [HttpGet]
        public IHttpActionResult GetAccount(long accountId)
        {
            return RedirectToEmployerAccountsApi();
        }

        [Route("{hashedAccountId}/users", Name = "GetAccountUsers")]
        [ApiAuthorize(Roles = "ReadAllAccountUsers")]
        [HttpGet]
        public IHttpActionResult GetAccountUsers(string hashedAccountId)
        {
            return RedirectToEmployerAccountsApi();
        }

        [Route("internal/{accountId}/users", Name = "GetAccountUsersByInternalAccountId")]
        [ApiAuthorize(Roles = "ReadAllAccountUsers")]
        [HttpGet]
        public IHttpActionResult GetAccountUsers(long accountId)
        {
            return RedirectToEmployerAccountsApi();
        }
    }
}
