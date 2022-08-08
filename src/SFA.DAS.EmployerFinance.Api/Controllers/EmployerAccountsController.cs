using SFA.DAS.EAS.Domain.Models.Account;
using SFA.DAS.EmployerFinance.Api.Orchestrators;
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

        [Route("")]        
        public async Task<IHttpActionResult> Index(BulkAccountsRequest accountIds)
        {
            var result = await _financeOrchestrator.GetAccountBalances(accountIds.AccountIds);

            if (result == null)
            {
                return NotFound();
            }

            return Ok(result);
        }

        [Route("balances")]
        [HttpPost]
        public async Task<IHttpActionResult> GetAccountBalances(AccountBalanceRequest accountIds)
        {
            var result = await _financeOrchestrator.GetAccountBalances(accountIds.HashedAccountIds);

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