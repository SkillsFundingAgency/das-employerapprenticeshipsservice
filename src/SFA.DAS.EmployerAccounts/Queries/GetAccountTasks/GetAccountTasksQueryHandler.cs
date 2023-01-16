using System.Threading;
using SFA.DAS.Common.Domain.Types;
using SFA.DAS.EmployerAccounts.Models.Account;
using SFA.DAS.Validation;
using TasksEnums = SFA.DAS.Tasks.API.Types.Enums;

namespace SFA.DAS.EmployerAccounts.Queries.GetAccountTasks;

public class GetAccountTasksQueryHandler : IRequestHandler<GetAccountTasksQuery, GetAccountTasksResponse>
{
    private readonly ITaskService _taskService;
    private readonly IValidator<GetAccountTasksQuery> _validator;

    public GetAccountTasksQueryHandler(ITaskService taskService, IValidator<GetAccountTasksQuery> validator)
    {
        _taskService = taskService;
        _validator = validator;
    }

    public async Task<GetAccountTasksResponse> Handle(GetAccountTasksQuery message, CancellationToken cancellationToken)
    {
        ValidateMessage(message);

        var accountTasks = await GetTasks(message);

        return new GetAccountTasksResponse
        {
            Tasks = accountTasks
        };
    }

    private async Task<AccountTask[]> GetTasks(GetAccountTasksQuery message)
    {
        var apprenticeshipEmployerType = 
            message.ApprenticeshipEmployerType == ApprenticeshipEmployerType.Levy 
                ? TasksEnums.ApprenticeshipEmployerType.Levy 
                : TasksEnums.ApprenticeshipEmployerType.NonLevy;

        var tasks = await _taskService.GetAccountTasks(message.AccountId, message.ExternalUserId, apprenticeshipEmployerType);

        var accountTasks = tasks.Select(x => new AccountTask
        {
            Type = x.Type.ToString(),
            ItemsDueCount = x.ItemsDueCount
        }).ToArray();

        return accountTasks;
    }

    private void ValidateMessage(GetAccountTasksQuery message)
    {
        var validationResults = _validator.Validate(message);

        if (!validationResults.IsValid())
        {
            throw new InvalidRequestException(validationResults.ValidationDictionary);
        }
    }
}