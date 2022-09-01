using SFA.DAS.EmployerFinance.Api.Orchestrators;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web.Http;

namespace SFA.DAS.EmployerFinance.Api.Controllers
{
    [RoutePrefix("api/accounts")]
    public class EmployerAccountsController : ApiController
    {
        private readonly FinanceOrchestrator _financeOrchestrator;

        public EmployerAccountsController(FinanceOrchestrator financeOrchestrator)
        {
            _financeOrchestrator = financeOrchestrator;
        }      

        [Route("balances")]
        [HttpPost]
        public async Task<IHttpActionResult> GetAccountBalances(List<string> accountIds)
        {
            var result = await _financeOrchestrator.GetAccountBalances(accountIds);

            if (result == null)
            {
                return NotFound();
            }

            return Ok(result);
        }

        [Route("{hashedAccountId}/transferAllowance")]
        public async Task<IHttpActionResult> GetTransferAllowance(string hashedAccountId)
        {
            var result = await _financeOrchestrator.GetTransferAllowance(hashedAccountId);

            if (result == null)
            {
                return NotFound();
            }

            return Ok(result);
        }
    }
}