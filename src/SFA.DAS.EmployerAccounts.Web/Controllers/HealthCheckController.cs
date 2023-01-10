﻿using SFA.DAS.Authorization.Mvc.Attributes;
using SFA.DAS.EmployerAccounts.Commands.RunHealthCheckCommand;
using SFA.DAS.EmployerAccounts.Queries.GetHealthCheck;

namespace SFA.DAS.EmployerAccounts.Web.Controllers;

[DasAuthorize]
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
        var response = await _mediator.SendAsync(query);
        var model = _mapper.Map<HealthCheckViewModel>(response);

        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Index(RunHealthCheckCommand command)
    {
        await _mediator.SendAsync(command);

        return RedirectToAction("Index");
    }
}