using System.Web.Http;
using SFA.DAS.EAS.Account.Api.Attributes;
using SFA.DAS.EAS.Domain.Configuration;

namespace SFA.DAS.EAS.Account.Api.Controllers
{
    [RoutePrefix("api/accounts/{hashedAccountId}/transactions")]
    public class AccountTransactionsController : RedirectController
    {
        public AccountTransactionsController(EmployerApprenticeshipsServiceConfiguration configuration) : base(configuration)
        {
        }

        [Route("", Name = "GetTransactionSummary")]
        [HttpGet]
        public IHttpActionResult Index(string hashedAccountId)
        {
            return RedirectToEmployerFinanceApi();
        }

        [Route("{year?}/{month?}", Name = "GetTransactions")]
        [ApiAuthorize(Roles = "ReadAllEmployerAccountBalances")]
        [HttpGet]
        public IHttpActionResult GetTransactions(string hashedAccountId, int year = 0, int month = 0)
        {
            return RedirectToEmployerFinanceApi();
        }
    }
}