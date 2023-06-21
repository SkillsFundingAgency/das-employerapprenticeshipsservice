using Microsoft.AspNetCore.Authorization;
using SFA.DAS.EAS.Web.Authentication;

namespace SFA.DAS.EAS.Web.Authorization;

public class EmployerUsersIsOutsideAccountAuthorizationHandler : AuthorizationHandler<EmployerAccountAllRolesRequirement>
{
    private readonly IEmployerAccountAuthorisationHandler _handler;

    public EmployerUsersIsOutsideAccountAuthorizationHandler(IEmployerAccountAuthorisationHandler handler)
    {
        _handler = handler;
    }

    protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, EmployerAccountAllRolesRequirement requirement)
    {
        if (!await _handler.IsOutsideAccount(context))
        {
            return;
        }

        context.Succeed(requirement);
    }
}
