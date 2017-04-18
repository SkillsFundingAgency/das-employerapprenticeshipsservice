using System;
using System.Threading.Tasks;
using System.Web.Http;
using SFA.DAS.EAS.Api.Attributes;
using SFA.DAS.EAS.Api.Orchestrators;

namespace SFA.DAS.EAS.Api.Controllers
{
    [RoutePrefix("api/accounts/{hashedAccountId}/levy")]
    public class AccountLevyController : ApiController
    {
        private readonly AccountsOrchestrator _orchestrator;

        public AccountLevyController(AccountsOrchestrator orchestrator)
        {
            _orchestrator = orchestrator;
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