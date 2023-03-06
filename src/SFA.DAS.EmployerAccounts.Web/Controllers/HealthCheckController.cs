using AutoMapper;
using SFA.DAS.EmployerAccounts.Commands.RunHealthCheckCommand;
using SFA.DAS.EmployerAccounts.Queries.GetHealthCheck;

namespace SFA.DAS.EmployerAccounts.Web.Controllers;

[Route("healthcheck")]
public class HealthCheckController : Controller
{
    private readonly IMediator _mediator;
    private readonly IMapper _mapper;

    public HealthCheckController(IMediator mediator, IMapper mapper)
    {
        _mediator = mediator;
        _mapper = mapper;
    }

    public async Task<IActionResult> Index(GetHealthCheckQuery query)
    {
        var response = await _mediator.Send(query);
        var model = _mapper.Map<HealthCheckViewModel>(response);

        return View(model);
    }

    [HttpPost]
    public async Task<IActionResult> Index(RunHealthCheckCommand command)
    {
        await _mediator.Send(command);

        return RedirectToAction("Index");
    }
}