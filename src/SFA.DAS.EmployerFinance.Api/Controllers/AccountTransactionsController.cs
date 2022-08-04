using SFA.DAS.EAS.Finance.Api.Types;
using SFA.DAS.EmployerFinance.Api.Attributes;
using SFA.DAS.EmployerFinance.Api.Orchestrators;
using System;
using System.Threading.Tasks;
using System.Web.Http;

namespace SFA.DAS.EmployerFinance.Api.Controllers
{
    [RoutePrefix("api/accounts/{hashedAccountId}/transactions")]
    public class AccountTransactionsController : ApiController
    {
        private readonly AccountTransactionsOrchestrator _orchestrator;

        public AccountTransactionsController(AccountTransactionsOrchestrator orchestrator)
        {
            _orchestrator = orchestrator;
        }

        [Route("", Name = "GetTransactionSummary")]
        [HttpGet]
        public async Task<IHttpActionResult> Index(string hashedAccountId)
        {
            var result = await _orchestrator.GetAccountTransactionSummary(hashedAccountId);

            if (result == null)
            {
                return NotFound();
            }

            result.ForEach(x => x.Href = Url.Route("GetTransactions", new { hashedAccountId, year = x.Year, month = x.Month }));

            return Ok(result);
        }

        [Route("{year?}/{month?}", Name = "GetTransactions")]
        [ApiAuthorize(Roles = "ReadAllEmployerAccountBalances")]
        [HttpGet]
        public async Task<IHttpActionResult> GetTransactions(string hashedAccountId, int year = 0, int month = 0)
        {
            var result = await GetAccountTransactions(hashedAccountId, year, month);

            if (result.Data == null)
            {
                return NotFound();
            }

            if (result.Data.HasPreviousTransactions)
            {
                var previousMonth = new DateTime(result.Data.Year, result.Data.Month, 1).AddMonths(-1);
                result.Data.PreviousMonthUri = Url.Route("GetTransactions", new { hashedAccountId, year = previousMonth.Year, month = previousMonth.Month });
            }

            return Ok(result.Data);
        }

        private async Task<OrchestratorResponse<TransactionsViewModel>> GetAccountTransactions(string hashedAccountId, int year, int month)
        {
            if (year == 0)
            {
                year = DateTime.Now.Year;
            }

            if (month == 0)
            {
                month = DateTime.Now.Month;
            }

            var result = await _orchestrator.GetAccountTransactions(hashedAccountId, year, month, Url);
            return result;
        }
    }
}