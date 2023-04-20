using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using SFA.DAS.EmployerAccounts.Infrastructure;
using SFA.DAS.EmployerAccounts.Web.Authentication;

namespace SFA.DAS.EmployerAccounts.Web.Controllers;

[Authorize(Policy = nameof(PolicyNames.HasEmployerViewerTransactorOwnerAccount))]
[Route("tasks/{hashedAccountId}")]
public class TaskController : BaseController
{
    private readonly TaskOrchestrator _orchestrator;
    private readonly ILogger<TaskController> _logger;
    public TaskController(
        TaskOrchestrator taskOrchestrator,
        ICookieStorageService<FlashMessageViewModel> flashMessage,
        ILogger<TaskController> logger)
        : base(flashMessage)
    {
        _orchestrator = taskOrchestrator;
        _logger = logger;
    }

    [HttpPost]
    [Route("dismissTask", Name = "DismissTask")]
    public async Task<IActionResult> DismissTask(DismissTaskViewModel viewModel)
    {
        if (string.IsNullOrEmpty(HttpContext.User.FindFirstValue(ControllerConstants.UserRefClaimKeyName)))
        {
            return RedirectToAction(ControllerConstants.IndexActionName, ControllerConstants.HomeControllerName);
        }

        var externalUserId = HttpContext.User.FindFirstValue(EmployerClaims.IdamsUserIdClaimTypeIdentifier);

        _logger.LogDebug("Task dismiss requested for account id '{HashedAccountId}', user id '{ExternalUserId}' and task '{TaskType}'", viewModel.HashedAccountId, externalUserId, viewModel.TaskType);

        var result = await _orchestrator.DismissMonthlyReminderTask(viewModel.HashedAccountId, externalUserId, viewModel.TaskType);

        if (result.Status != HttpStatusCode.OK)
        {
            //Curently we are not telling the user of the error and are instead just logging the issue
            //The error log will be done at a lower level
            _logger.LogDebug("Task dismiss requested failed for account id '{HashedAccountId}', user id '{ExternalUserId}' and task '{TaskType}'", viewModel.HashedAccountId, externalUserId, viewModel.TaskType);
        }

        return RedirectToAction("Index", "EmployerTeam");
    }
}