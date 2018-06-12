using System.Threading.Tasks;
using System.Web.Http;
using MediatR;
using SFA.DAS.EAS.Account.Api.Attributes;
using SFA.DAS.EAS.Application.Queries.GetStatistics;

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
            var response = await _mediator.SendAsync(new GetStatisticsQuery());
            return Ok(response.Statistics);
        }
    }
}
