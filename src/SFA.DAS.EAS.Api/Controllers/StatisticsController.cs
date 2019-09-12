using System.Threading.Tasks;
using System.Web.Http;
using SFA.DAS.EAS.Account.Api.Attributes;
using SFA.DAS.EAS.Account.Api.Orchestrators;

namespace SFA.DAS.EAS.Account.Api.Controllers
{
    [ApiAuthorize(Roles = "ReadUserAccounts")]
    [RoutePrefix("api/statistics")]
    public class StatisticsController : ApiController
    {
        private readonly StatisticsOrchestrator _statisticsOrchestrator;

        public StatisticsController(StatisticsOrchestrator statisticsOrchestrator)
        {
            _statisticsOrchestrator = statisticsOrchestrator;
        }

        [Route("")]
        public async Task<IHttpActionResult> GetStatistics()
        {
            return Ok(await _statisticsOrchestrator.Get());
        }
    }
}
