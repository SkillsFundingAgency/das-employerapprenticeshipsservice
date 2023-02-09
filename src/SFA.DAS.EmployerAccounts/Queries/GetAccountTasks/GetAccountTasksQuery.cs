
using SFA.DAS.Common.Domain.Types;

namespace SFA.DAS.EmployerAccounts.Queries.GetAccountTasks;

public class GetAccountTasksQuery : IRequest<GetAccountTasksResponse>
{
    public long AccountId { get; set; }
    public string ExternalUserId { get; set; }
    public ApprenticeshipEmployerType ApprenticeshipEmployerType { get; set; }
}