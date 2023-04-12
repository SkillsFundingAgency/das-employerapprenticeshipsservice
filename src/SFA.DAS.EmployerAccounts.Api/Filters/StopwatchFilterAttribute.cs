using System.Diagnostics;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;

namespace SFA.DAS.EmployerAccounts.Api.Filters;

public class StopwatchFilterAttribute : ActionFilterAttribute
{
    private readonly ILogger<StopwatchFilterAttribute> _logger;
    private Stopwatch _stopWatch;
    private const int WarningThreshold = 5000;

    public StopwatchFilterAttribute(ILogger<StopwatchFilterAttribute> logger)
    {
        _logger = logger;
    }

    public override void OnActionExecuting(ActionExecutingContext context)
    {
        _stopWatch = new Stopwatch();
        _stopWatch.Start();
    }

    public override void OnActionExecuted(ActionExecutedContext context)
    {
        _stopWatch.Stop();

        if (_stopWatch.ElapsedMilliseconds <= WarningThreshold) return;

        var controllerName = context.RouteData.Values["controller"];
        var actionName = context.RouteData.Values["action"];
        _logger.LogWarning($"Controller action took {_stopWatch.ElapsedMilliseconds} to complete: {controllerName}.{actionName}");
    }
}