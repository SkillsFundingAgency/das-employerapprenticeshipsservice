using System.Threading;
using SFA.DAS.HashingService;
using SFA.DAS.Validation;

namespace SFA.DAS.EmployerAccounts.Commands.DismissMonthlyTaskReminder;

public class DismissMonthlyTaskReminderCommandHandler : IRequestHandler<DismissMonthlyTaskReminderCommand>
{
    private readonly ITaskService _taskService;
    private readonly IValidator<DismissMonthlyTaskReminderCommand> _validator;
    private readonly IHashingService _hashingService;

    public DismissMonthlyTaskReminderCommandHandler(
        ITaskService taskService, 
        IValidator<DismissMonthlyTaskReminderCommand> validator, 
        IHashingService hashingService)
    {
        _taskService = taskService;
        _validator = validator;
        _hashingService = hashingService;
    }

    public async Task<Unit> Handle(DismissMonthlyTaskReminderCommand request, CancellationToken cancellationToken)
    {
        var validationResults = await _validator.ValidateAsync(request);

        if (!validationResults.IsValid())
        {
            throw new InvalidRequestException(validationResults.ValidationDictionary);
        }

        var accountId = _hashingService.DecodeValue(request.HashedAccountId);
          
        await _taskService.DismissMonthlyTaskReminder(accountId, request.ExternalUserId, request.TaskType);

        return default;
    }
}