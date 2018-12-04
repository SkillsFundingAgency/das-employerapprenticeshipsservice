using System;
using SFA.DAS.EmployerAccounts.ReadStore.Mediator;

namespace SFA.DAS.EmployerAccounts.ReadStore.Application.Commands
{
    internal class RemoveAccountUserCommand : IReadStoreRequest<Unit>
    {
        public long AccountId { get; }
        public Guid UserRef { get; }
        public string MessageId { get; }
        public DateTime Removed { get; }

        public RemoveAccountUserCommand(long accountId, Guid userRef, string messageId, DateTime removed)
        {
            AccountId = accountId;
            UserRef = userRef;
            MessageId = messageId;
            Removed = removed;
        }
    }
}
