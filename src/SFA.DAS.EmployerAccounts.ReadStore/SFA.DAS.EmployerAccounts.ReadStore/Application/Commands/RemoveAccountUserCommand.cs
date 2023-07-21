using System;
using MediatR;

namespace SFA.DAS.EmployerAccounts.ReadStore.Application.Commands;

internal class RemoveAccountUserCommand : IRequest
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