using System;
using System.Threading.Tasks;
using System.Web.Http;
using SFA.DAS.EAS.Api.Attributes;
using SFA.DAS.EAS.Api.Orchestrators;

namespace SFA.DAS.EAS.Api.Controllers
{
    [RoutePrefix("api/accounts/{hashedAccountId}/transactions")]
    public class AccountTransactionsController : ApiController
    {
        private readonly AccountTransactionsOrchestrator _orchestrator;

        public AccountTransactionsController(AccountTransactionsOrchestrator orchestrator)
        {
            _orchestrator = orchestrator;
        }

        [Route("{year?}/{month?}", Name = "GetTransactions")]
        [ApiAuthorize(Roles = "ReadAllEmployerAccountBalances")]
        [HttpGet]
        public async Task<IHttpActionResult> Index(string hashedAccountId, int year, int month)
        {
            var result = await _orchestrator.GetAccountTransactions(hashedAccountId, year, month);

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
    }
}