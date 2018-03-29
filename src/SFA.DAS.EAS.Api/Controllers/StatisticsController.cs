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
        private readonly IStatisticsOrchestrator _orchestrator;

        public StatisticsController(IStatisticsOrchestrator orchestrator)
        {
            _orchestrator = orchestrator;
        }

        [HttpGet]
        [Route("")]
        public async Task<IHttpActionResult> GetStatistics()
        {
            var model = await _orchestrator.GetStatistics();

            if (model.Data == null)
            {
                return NotFound();
            }

            return Ok(model.Data);
        }
    }
}
