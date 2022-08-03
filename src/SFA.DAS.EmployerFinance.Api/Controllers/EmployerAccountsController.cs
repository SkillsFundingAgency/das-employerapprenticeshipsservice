using SFA.DAS.EmployerFinance.Api.Orchestrators;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web.Http;

namespace SFA.DAS.EmployerFinance.Api.Controllers
{
    [RoutePrefix("api/accounts/balances")]
    public class EmployerAccountsController : ApiController
    {
        private readonly FinanceOrchestrator _financeOrchestrator;

        public EmployerAccountsController(FinanceOrchestrator financeOrchestrator)
        {
            _financeOrchestrator = financeOrchestrator;
        }

        [Route("{accountIds}", Name= "GetAccountBalances")]
        public async Task<IHttpActionResult> GetAccountBalances(List<long> accountIds)
        {
            return Ok(await _financeOrchestrator.GetAccountBalances(accountIds));
        }
    }
}