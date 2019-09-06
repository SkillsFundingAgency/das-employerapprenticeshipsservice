using MediatR;

namespace SFA.DAS.EmployerAccounts.Commands.AccountLevyStatus
{
    public class UpdateAccountToLevy : IAsyncRequest
    {
        public UpdateAccountToLevy(long accountId)
        {
            AccountId = accountId;
        }

        public long AccountId { get; }
    }
}