using SFA.DAS.EmployerAccounts.Commands.DismissMonthlyTaskReminder;
using SFA.DAS.Tasks.API.Types.Enums;

namespace SFA.DAS.EmployerAccounts.Web.Orchestrators;

public class TaskOrchestrator
{
    private readonly IMediator _mediator;
    private readonly ILog _logger;

    public TaskOrchestrator(IMediator mediator, ILog logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    public async Task<OrchestratorResponse> DismissMonthlyReminderTask(string hashedAccountId, string externalUserId, string taskTypeName)
    {
        try
        {
            TaskType taskType;

            _logger.Debug($"Dismissing task reminder {taskTypeName} for account id {hashedAccountId} and user id {externalUserId}");

            if (!Enum.TryParse(taskTypeName, out taskType))
            {
                _logger.Warn(
                    $"Invalid task name for account (account id: {hashedAccountId}, user id: {externalUserId}, Task type: {taskTypeName}");
                return new OrchestratorResponse { Status = HttpStatusCode.BadRequest };
            }

            await _mediator.Send(new DismissMonthlyTaskReminderCommand
            {
                HashedAccountId = hashedAccountId,
                ExternalUserId = externalUserId,
                TaskType = taskType
            });
        }
        catch (InvalidRequestException ire)
        {
            _logger.Warn(ire, $"Invalid request for account (account id: {hashedAccountId}, user id: {externalUserId}, Task type: {taskTypeName}");
            return new OrchestratorResponse { Status = HttpStatusCode.BadRequest};
        }
        catch (Exception ex)
        {
            _logger.Error(ex, $"Error occurred when dismissing a task reminder (account id: {hashedAccountId}, user id: {externalUserId}, Task type: {taskTypeName}");
            return new OrchestratorResponse { Status = HttpStatusCode.InternalServerError, Exception = ex};
        }

        return new OrchestratorResponse{Status = HttpStatusCode.OK};
    }
}