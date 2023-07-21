using System;
using MediatR;

namespace SFA.DAS.EmployerAccounts.ReadStore.Application.Queries;

internal class IsUserInAnyRoleQuery : IRequest<bool>
{
    public Guid UserRef { get; }
    public long AccountId { get; }

    public IsUserInAnyRoleQuery(Guid userRef, long accountId)
    {
        UserRef = userRef;
        AccountId = accountId;
    }
}