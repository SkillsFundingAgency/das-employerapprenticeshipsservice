using System.Threading.Tasks;
using System.Web.Http;
using MediatR;
using SFA.DAS.EmployerAccounts.Api.Attributes;
using SFA.DAS.EmployerAccounts.Queries.GetStatistics;

namespace SFA.DAS.EmployerAccounts.Api.Controllers
{
    [ApiAuthorize(Roles = "ReadUserAccounts")]
    [RoutePrefix("api/statistics")]
    public class StatisticsController : Microsoft.AspNetCore.Mvc.ControllerBase
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
