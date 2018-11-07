using System.Threading.Tasks;
using System.Web.Http;
using MediatR;
using SFA.DAS.EmployerFinance.Attributes;
using SFA.DAS.EmployerFinance.Queries.GetStatistics;

namespace SFA.DAS.EmployerFinance.Api.Controllers
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
