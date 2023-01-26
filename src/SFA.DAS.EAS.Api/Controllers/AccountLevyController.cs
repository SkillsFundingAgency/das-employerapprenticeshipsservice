using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SFA.DAS.EAS.Account.Api.Orchestrators;

namespace SFA.DAS.EAS.Account.Api.Controllers
{
    [ApiController]
    [Route("api/accounts/{hashedAccountId}/levy")]
    public class AccountLevyController : ControllerBase
    {
        private readonly AccountsOrchestrator _orchestrator;

        public AccountLevyController(AccountsOrchestrator orchestrator)
        {
            _orchestrator = orchestrator;
        }

        [Authorize(Policy = "LoopBack", Roles = "ReadAllEmployerAccountBalances")]
        [HttpGet(Name = "GetLevy")]
        public async Task<IActionResult> Index(string hashedAccountId)
        {
            var result = await _orchestrator.GetLevy(hashedAccountId);

            if (result.Data == null)
            {
                return NotFound();
            }

            return Ok(result.Data);
        }

        [Authorize(Policy = "LoopBack", Roles = "ReadAllEmployerAccountBalances")]
        [HttpGet("{payrollYear}/{payrollMonth}", Name = "GetLevyForPeriod")]
        public async Task<IActionResult> GetLevy(string hashedAccountId, string payrollYear, short payrollMonth)
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