using System.Threading;
using SFA.DAS.Encoding;

namespace SFA.DAS.EmployerAccounts.Commands.DismissMonthlyTaskReminder;

public class DismissMonthlyTaskReminderCommandHandler : IRequestHandler<DismissMonthlyTaskReminderCommand>
{
    private readonly ITaskService _taskService;
    private readonly IValidator<DismissMonthlyTaskReminderCommand> _validator;
    private readonly IEncodingService _encodingService;

    public DismissMonthlyTaskReminderCommandHandler(
        ITaskService taskService, 
        IValidator<DismissMonthlyTaskReminderCommand> validator, 
        IEncodingService encodingService)
    {
        _taskService = taskService;
        _validator = validator;
        _encodingService = encodingService;
    }

    public async Task<Unit> Handle(DismissMonthlyTaskReminderCommand request, CancellationToken cancellationToken)
    {
        var validationResults = _validator.Validate(request);

        if (!validationResults.IsValid())
        {
            throw new InvalidRequestException(validationResults.ValidationDictionary);
        }

        var accountId = _encodingService.Decode(request.HashedAccountId, EncodingType.AccountId);
          
        await _taskService.DismissMonthlyTaskReminder(accountId, request.ExternalUserId, request.TaskType);

        return Unit.Value;
    }
}