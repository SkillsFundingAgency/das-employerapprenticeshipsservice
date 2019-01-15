using System;
using SFA.DAS.EmployerAccounts.ReadStore.Mediator;
using SFA.DAS.EmployerAccounts.Types.Models;

namespace SFA.DAS.EmployerAccounts.ReadStore.Application.Commands
{
    internal class CreateAccountUserCommand : IReadStoreRequest<Unit>
    {
        public long AccountId { get; }
        public Guid UserRef { get; }
        public UserRole Role { get; }
        public string MessageId { get; }
        public DateTime Created { get; }

        public CreateAccountUserCommand(long accountId, Guid userRef, UserRole role, string messageId, DateTime created)
        {
            AccountId = accountId;
            UserRef = userRef;
            Role = role;
            MessageId = messageId;
            Created = created;
        }
    }
}
