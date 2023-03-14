using Microsoft.AspNetCore.Authorization;
using SFA.DAS.EmployerAccounts.Web.Authentication;

namespace SFA.DAS.EmployerAccounts.Web.Authorization;

public class EmployerUserHasNoAccountAuthorizationHandler : AuthorizationHandler<EmployerAccountAllRolesRequirement>
{
    private readonly IEmployerAccountAuthorisationHandler _handler;

    public EmployerUserHasNoAccountAuthorizationHandler(IEmployerAccountAuthorisationHandler handler)
    {
        _handler = handler;
    }

    protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, EmployerAccountAllRolesRequirement requirement)
    {
        if (!(await _handler.IsUserCreatingAccount(context)))
        {
            return;
        }

        context.Succeed(requirement);

        return;
    }
}
