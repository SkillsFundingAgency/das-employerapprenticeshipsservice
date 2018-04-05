using System;
using System.Linq;
using System.Threading.Tasks;
using MediatR;
using SFA.DAS.EAS.Application.Exceptions;
using SFA.DAS.EAS.Application.Validation;
using SFA.DAS.EAS.Domain.Interfaces;
using SFA.DAS.EAS.Domain.Models.Account;

namespace SFA.DAS.EAS.Application.Queries.GetAccountTasks
{
    public class GetAccountTasksQueryHandler : IAsyncRequestHandler<GetAccountTasksQuery, GetAccountTasksResponse>
    {
        private readonly ITaskService _taskService;
        private readonly IValidator<GetAccountTasksQuery> _validator;

        public GetAccountTasksQueryHandler(ITaskService taskService, IValidator<GetAccountTasksQuery> validator)
        {
            _taskService = taskService;
            _validator = validator;
        }

        public async Task<GetAccountTasksResponse> Handle(GetAccountTasksQuery message)
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
            var tasks = await _taskService.GetAccountTasks(message.AccountId, message.ExternalUserId);

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
}
