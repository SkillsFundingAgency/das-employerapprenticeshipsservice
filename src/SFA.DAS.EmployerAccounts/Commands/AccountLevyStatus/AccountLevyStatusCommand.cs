using SFA.DAS.Common.Domain.Types;

namespace SFA.DAS.EmployerAccounts.Commands.AccountLevyStatus;

public class AccountLevyStatusCommand : IAsyncRequest
{
    public long AccountId { get; set; }
    public ApprenticeshipEmployerType ApprenticeshipEmployerType { get; set; }
}