using System.Threading.Tasks;
using System.Web.Http;
using MediatR;
using SFA.DAS.EAS.Account.Api.Attributes;
using SFA.DAS.EAS.Application.Queries.GetFinancialStatistics;

namespace SFA.DAS.EAS.Account.Api.Controllers
{
    [ApiAuthorize(Roles = "ReadUserAccounts")]
    [RoutePrefix("api/statistics")]
    public class StatisticsController : ApiController
    {
        private readonly IMediator _mediator;

        public StatisticsController(IMediator mediator)
        {
            _mediator = mediator;
        }
        
        [Route("")]
        public async Task<IHttpActionResult> GetStatistics()
        {
            //todo: add GetStatistics call to employeraccounts api client?
            // inject client into controller (or inject client into new statistics orchestrator, or wrap client in service and inject into orch/controller)

            var financialStatisticsTask = _mediator.SendAsync(new GetFinancialStatisticsQuery());
            var financialStatistics = await financialStatisticsTask;
            return Ok(financialStatistics.Statistics);
        }
    }
}
