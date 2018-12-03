using System;
using SFA.DAS.EmployerAccounts.ReadStore.Mediator;

namespace SFA.DAS.EmployerAccounts.ReadStore.Application.Commands
{
    internal class RemoveAccountUserCommand : IReadStoreRequest<Unit>
    {
        public long AccountId { get; }
        public long UserId { get; }
        public string MessageId { get; }
        public DateTime Removed { get; }

        public RemoveAccountUserCommand(long accountId, long userId, string messageId, DateTime removed)
        {
            AccountId = accountId;
            UserId = userId;
            MessageId = messageId;
            Removed = removed;
        }
    }
}
