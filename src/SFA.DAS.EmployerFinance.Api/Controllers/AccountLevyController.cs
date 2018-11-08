using System.Threading.Tasks;
using System.Web.Http;
using SFA.DAS.EmployerFinance.Api.Orchestrator;
using SFA.DAS.EmployerFinance.Attributes;

namespace SFA.DAS.EmployerFinance.Api.Controllers
{
    [RoutePrefix("api/accounts/{hashedAccountId}/levy")]
    public class AccountLevyController : ApiController
    {
        private readonly AccountsOrchestrator _orchestrator;

        public AccountLevyController(AccountsOrchestrator orchestrator)
        {
            _orchestrator = orchestrator;
        }

        [Route("", Name = "GetLevy")]
        [ApiAuthorize(Roles = "ReadAllEmployerAccountBalances")]
        [HttpGet]
        public async Task<IHttpActionResult> Index(string hashedAccountId)
        {
            var result = await _orchestrator.GetLevy(hashedAccountId);

            if (result.Data == null)
            {
                return NotFound();
            }

            return Ok(result.Data);
        }

        [Route("{payrollYear}/{payrollMonth}", Name = "GetLevyForPeriod")]
        [ApiAuthorize(Roles = "ReadAllEmployerAccountBalances")]
        [HttpGet]
        public async Task<IHttpActionResult> GetLevy(string hashedAccountId, string payrollYear, short payrollMonth)
        {
            var result = await _orchestrator.GetLevy(hashedAccountId, payrollYear, payrollMonth);

            if (result.Data == null)
            {
                return NotFound();
            }

            return Ok(result.Data);
        }
    }
}