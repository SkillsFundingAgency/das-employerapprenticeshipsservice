using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SFA.DAS.EmployerAccounts.Api.Authorization;
using SFA.DAS.EmployerAccounts.Queries.GetStatistics;

namespace SFA.DAS.EmployerAccounts.Api.Controllers;

[Authorize(Policy = ApiRoles.ReadUserAccounts)]
[Route("api/statistics")]
public class StatisticsController : ControllerBase
{
    private readonly IMediator _mediator;

    public StatisticsController(IMediator mediator) => _mediator = mediator;

    [HttpGet]
    [Route("")]
    public async Task<IActionResult> GetStatistics()
    {
        var response = await _mediator.Send(new GetStatisticsQuery());
        return Ok(response.Statistics);
    }
}