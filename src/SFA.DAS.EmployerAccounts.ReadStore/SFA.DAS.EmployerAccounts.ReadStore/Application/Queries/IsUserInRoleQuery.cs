using System;
using System.Collections.Generic;
using MediatR;
using SFA.DAS.EmployerAccounts.Types.Models;

namespace SFA.DAS.EmployerAccounts.ReadStore.Application.Queries;

public class IsUserInRoleQuery : IRequest<bool>
{
    public Guid UserRef { get; }
    public long AccountId { get; }
    public HashSet<UserRole> UserRoles { get; }

    public IsUserInRoleQuery(Guid userRef, long accountId, HashSet<UserRole> userRoles)
    {
        UserRef = userRef;
        AccountId = accountId;
        UserRoles = userRoles;
    }
}