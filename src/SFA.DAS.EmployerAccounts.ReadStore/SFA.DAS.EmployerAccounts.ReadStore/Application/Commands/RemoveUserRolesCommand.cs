using System;
using SFA.DAS.EmployerAccounts.ReadStore.Mediator;

namespace SFA.DAS.EmployerAccounts.ReadStore.Application.Commands
{
    internal class RemoveUserRolesCommand : IReadStoreRequest<Unit>
    {
        public long AccountId { get; }
        public long UserId { get; }
        public string MessageId { get; }
        public DateTime Updated { get; }

        public RemoveUserRolesCommand(long accountId, long userId, string messageId, DateTime updated)
        {
            AccountId = accountId;
            UserId = userId;
            MessageId = messageId;
            Updated = updated;
        }
    }
}
