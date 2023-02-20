using Microsoft.AspNetCore.Authorization;
using SFA.DAS.EmployerAccounts.Web.Authentication;

namespace SFA.DAS.EmployerAccounts.Web.Authorization;

public class EmployerAccountOwnerAuthorizationHandler : AuthorizationHandler<EmployerAccountOwnerRequirement>
{
    private readonly IEmployerAccountAuthorisationHandler _handler;

    public EmployerAccountOwnerAuthorizationHandler(IEmployerAccountAuthorisationHandler handler)
    {
        _handler = handler;
    }

    protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, EmployerAccountOwnerRequirement ownerRequirement)
    {
        if (!(await _handler.IsEmployerAuthorised(context, false)))
        {
            return;
        }

        context.Succeed(ownerRequirement);
    }
}

