using System;
using System.Net;
using System.Threading.Tasks;
using MediatR;
using SFA.DAS.EAS.Application;
using SFA.DAS.EAS.Application.Commands.DismissMonthlyTaskReminder;
using SFA.DAS.NLog.Logger;
using SFA.DAS.Tasks.API.Types.Enums;

namespace SFA.DAS.EAS.Web.Orchestrators
{
    public class TaskOrchestrator
    {
        private readonly IMediator _mediator;
        private readonly ILog _logger;

        public TaskOrchestrator(IMediator mediator, ILog logger)
        {
            _mediator = mediator;
            _logger = logger;
        }

        public async Task<OrchestratorResponse> DismissMonthlyReminderTask(string hashedAccountId, string hashedUserId, TaskType taskType)
        {
            _logger.Debug($"Dismissing task reminder {Enum.GetName(typeof(TaskType), taskType)} for account id {hashedAccountId} and user id {hashedUserId}");

            try
            {
                await _mediator.SendAsync(new DismissMonthlyTaskReminderCommand
                {
                    HashedAccountId = hashedAccountId,
                    HashedUserId = hashedUserId,
                    TaskType = taskType
                });
            }
            catch (InvalidRequestException ire)
            {
                _logger.Warn(ire, $"Invalid request for account (account id: {hashedAccountId}, user id: {hashedUserId}, Task type: {taskType}");
                return new OrchestratorResponse { Status = HttpStatusCode.BadRequest};
            }
            catch (Exception ex)
            {
                _logger.Error(ex, $"Error occured when dismissing a task reminder (account id: {hashedAccountId}, user id: {hashedUserId}, Task type: {Enum.GetName(typeof(TaskType), taskType)}");
                return new OrchestratorResponse { Status = HttpStatusCode.InternalServerError, Exception = ex};
            }

            return new OrchestratorResponse{Status = HttpStatusCode.OK};
        }
    }
}