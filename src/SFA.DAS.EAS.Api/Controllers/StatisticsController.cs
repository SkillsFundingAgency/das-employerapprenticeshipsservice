using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SFA.DAS.EAS.Account.Api.Orchestrators;

namespace SFA.DAS.EAS.Account.Api.Controllers
{
    [ApiController]
    [Authorize(Policy = "LoopBack", Roles = "ReadUserAccounts")]
    [Route("api/statistics")]
    public class StatisticsController : ControllerBase
    {
        private readonly StatisticsOrchestrator _statisticsOrchestrator;

        public StatisticsController(StatisticsOrchestrator statisticsOrchestrator)
        {
            _statisticsOrchestrator = statisticsOrchestrator;
        }

        [HttpGet]
        public async Task<ActionResult<Types.StatisticsViewModel>> GetStatistics()
        {
            return Ok(await _statisticsOrchestrator.Get());
        }
    }
}
