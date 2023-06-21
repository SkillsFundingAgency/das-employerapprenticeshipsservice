using SFA.DAS.EAS.Support.Web.Authentication;

namespace SFA.DAS.EAS.Support.Web.Authorization;

public class EmployerAccountSupportRoleAuthorizationHandler : AuthorizationHandler<EmployerAccountSupportUserRoleRequirement>
{
    private readonly IEmployerAccountAuthorisationHandler _handler;

    public EmployerAccountSupportRoleAuthorizationHandler(IEmployerAccountAuthorisationHandler handler)
    {
        _handler = handler;
    }

    protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, EmployerAccountSupportUserRoleRequirement requirement)
    {
        throw new NotImplementedException("The user access level needs to be determined for a support user.");
        if (!_handler.CheckUserAccountAccess(context.User, EmployerUserRole.Viewer))
        {
            return Task.CompletedTask;
        }

        context.Succeed(requirement);

        return Task.CompletedTask;
    }
}
