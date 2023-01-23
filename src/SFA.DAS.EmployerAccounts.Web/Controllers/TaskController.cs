using System.Security.Claims;
using SFA.DAS.Authorization.Mvc.Attributes;

namespace SFA.DAS.EmployerAccounts.Web.Controllers;

[DasAuthorize]
[Route("tasks/{hashedAccountId}")]
public class TaskController : BaseController
{
    private readonly TaskOrchestrator _orchestrator;
    private readonly ILogger<TaskController> _logger;
    private readonly HttpContextAccessor _contextAccessor;

    public TaskController(
        TaskOrchestrator taskOrchestrator,
        ICookieStorageService<FlashMessageViewModel> flashMessage,
        ILogger<TaskController> logger,
        HttpContextAccessor contextAccessor
        ) : base(flashMessage)
    {
        _orchestrator = taskOrchestrator;
        _logger = logger;
        _contextAccessor = contextAccessor;
    }

    [HttpPost]
    [DasAuthorize]
    [Route("dismissTask", Name = "DismissTask")]
    public async Task<IActionResult> DismissTask(DismissTaskViewModel viewModel)
    {
        if (string.IsNullOrEmpty(_contextAccessor.HttpContext.User.FindFirstValue(ControllerConstants.UserRefClaimKeyName)))
        {
            return RedirectToAction(ControllerConstants.IndexActionName, ControllerConstants.HomeControllerName);
        }

        var externalUserId = _contextAccessor.HttpContext.User.FindFirstValue("sub");

        _logger.LogDebug($"Task dismiss requested for account id '{viewModel.HashedAccountId}', user id '{externalUserId}' and task '{viewModel.TaskType}'");

        var result = await _orchestrator.DismissMonthlyReminderTask(viewModel.HashedAccountId, externalUserId, viewModel.TaskType);

        if (result.Status != HttpStatusCode.OK)
        {
            //Curently we are not telling the user of the error and are instead just logging the issue
            //The error log will be done at a lower level
            _logger.LogDebug($"Task dismiss requested failed for account id '{viewModel.HashedAccountId}', user id '{externalUserId}' and task '{viewModel.TaskType}'");
        }

        return RedirectToAction("Index", "EmployerTeam");
    }
}