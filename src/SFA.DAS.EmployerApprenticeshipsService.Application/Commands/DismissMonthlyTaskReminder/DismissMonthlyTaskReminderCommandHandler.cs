using System.Threading.Tasks;
using MediatR;
using SFA.DAS.EAS.Application.Validation;
using SFA.DAS.EAS.Domain.Interfaces;
using SFA.DAS.NLog.Logger;

namespace SFA.DAS.EAS.Application.Commands.DismissMonthlyTaskReminder
{
    public class DismissMonthlyTaskReminderCommandHandler : AsyncRequestHandler<DismissMonthlyTaskReminderCommand>
    {
        private readonly ITaskService _taskService;
        private readonly IValidator<DismissMonthlyTaskReminderCommand> _validator;
        private readonly ILog _logger;

        public DismissMonthlyTaskReminderCommandHandler(
            ITaskService taskService, 
            IValidator<DismissMonthlyTaskReminderCommand> validator, 
            ILog logger)
        {
            _taskService = taskService;
            _validator = validator;
            _logger = logger;
        }

        protected override async Task HandleCore(DismissMonthlyTaskReminderCommand command)
        {
            var validationResults = _validator.Validate(command);

            if (!validationResults.IsValid())
            {
                throw new InvalidRequestException(validationResults.ValidationDictionary);
            }

            await _taskService.DismissMonthlyTaskReminder(command.HashedAccountId, command.HashedUserId, command.TaskType);
        }
    }
}
