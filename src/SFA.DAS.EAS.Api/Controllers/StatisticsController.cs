using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using SFA.DAS.EAS.Account.Api.Orchestrators;
using SFA.DAS.EAS.Account.Api.Types;

namespace SFA.DAS.EAS.Account.Api.Controllers
{
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
        public async Task<IHttpActionResult> GetTheRdsStatistics()
        {
            var model = await _orchestrator.GetTheRdsRequiredStatistics();

            if (model.Data == null)
            {
                return NotFound();
            }

            return Ok(model.Data);
        }
    }
}
