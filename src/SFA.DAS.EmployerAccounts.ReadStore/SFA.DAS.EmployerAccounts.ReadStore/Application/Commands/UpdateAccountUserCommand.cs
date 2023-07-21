using System;
using MediatR;
using SFA.DAS.EmployerAccounts.Types.Models;

namespace SFA.DAS.EmployerAccounts.ReadStore.Application.Commands;

internal class UpdateAccountUserCommand : IRequest
{
    public long AccountId { get; }
    public Guid UserRef { get; }
    public UserRole Role { get; }
    public string MessageId { get; }
    public DateTime Updated { get; }

    public UpdateAccountUserCommand(long accountId, Guid userRef, UserRole role, string messageId, DateTime updated)
    {
        AccountId = accountId;
        UserRef = userRef;
        Role = role;
        MessageId = messageId;
        Updated = updated;
    }
}