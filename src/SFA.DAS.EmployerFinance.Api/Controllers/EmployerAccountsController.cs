using SFA.DAS.EAS.Domain.Models.Account;
using SFA.DAS.EmployerFinance.Api.Orchestrators;
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

        [Route("")]        
        public async Task<IHttpActionResult> Index(BulkAccountsRequest accountIds) //TODO : change to hashedAccountIds
        {
            var result = await _financeOrchestrator.GetAccountBalances(accountIds.AccountIds);

            if (result == null)
            {
                return NotFound();
            }

            return Ok(result);
        }

        [Route("{accountId}/transferAllowance")]
        public async Task<IHttpActionResult> GetTransferAllowance(long accountId) //TODO : change to hashedAccountId
        {
            var result = await _financeOrchestrator.GetTransferAllowance(accountId);

            if (result == null)
            {
                return NotFound();
            }

            return Ok(result);
        }
    }
}