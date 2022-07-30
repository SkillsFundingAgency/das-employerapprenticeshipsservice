using SFA.DAS.EmployerFinance.Api.Attributes;
using SFA.DAS.EmployerFinance.Api.Orchestrators;
using System.Threading.Tasks;
using System.Web.Http;

namespace SFA.DAS.EmployerFinance.Api.Controllers
{
    [ApiAuthorize(Roles = "ReadUserAccounts")]
    [RoutePrefix("api/financestatistics")]
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
