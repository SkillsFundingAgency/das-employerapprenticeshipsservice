using SFA.DAS.EmployerFinance.Api.Attributes;
using SFA.DAS.EmployerFinance.Api.Orchestrators;
using System.Threading.Tasks;
using System.Web.Http;

namespace SFA.DAS.EmployerFinance.Api.Controllers
{
    [RoutePrefix("api/accounts/{hashedAccountId}/levy")]
    public class FinanceLevyController : ApiController
    {
        private readonly FinanceOrchestrator _orchestrator;

        public FinanceLevyController(FinanceOrchestrator orchestrator)
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
    }
}