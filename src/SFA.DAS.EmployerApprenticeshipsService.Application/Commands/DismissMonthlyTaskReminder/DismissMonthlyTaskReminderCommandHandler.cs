using System.Threading.Tasks;
using MediatR;
using SFA.DAS.Validation;
using SFA.DAS.EAS.Domain.Interfaces;
using SFA.DAS.HashingService;
using SFA.DAS.NLog.Logger;

namespace SFA.DAS.EAS.Application.Commands.DismissMonthlyTaskReminder
{
    public class DismissMonthlyTaskReminderCommandHandler : AsyncRequestHandler<DismissMonthlyTaskReminderCommand>
    {
        private readonly ITaskService _taskService;
        private readonly IValidator<DismissMonthlyTaskReminderCommand> _validator;
        private readonly ILog _logger;
        private readonly IHashingService _hashingService;

        public DismissMonthlyTaskReminderCommandHandler(
            ITaskService taskService, 
            IValidator<DismissMonthlyTaskReminderCommand> validator, 
            ILog logger, 
            IHashingService hashingService)
        {
            _taskService = taskService;
            _validator = validator;
            _logger = logger;
            _hashingService = hashingService;
        }

        protected override async Task HandleCore(DismissMonthlyTaskReminderCommand command)
        {
            var validationResults = _validator.Validate(command);

            if (!validationResults.IsValid())
            {
                throw new InvalidRequestException(validationResults.ValidationDictionary);
            }

            var accountId = _hashingService.DecodeValue(command.HashedAccountId);
          
            await _taskService.DismissMonthlyTaskReminder(accountId, command.ExternalUserId, command.TaskType);
        }
    }
}
