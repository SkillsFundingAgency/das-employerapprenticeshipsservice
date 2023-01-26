using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Threading.Tasks;

using SFA.DAS.EAS.Account.Api.Extensions;
using Microsoft.AspNetCore.Http;

namespace SFA.DAS.EAS.Account.Api.AuthPolicies;

public class LoopBackRequirement : IAuthorizationRequirement { }


public class LoopBackHandler : AuthorizationHandler <LoopBackRequirement>
{

    public LoopBackHandler() { }
    protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, LoopBackRequirement requirement)
    {
        if (context.Resource is not HttpContext httpContext)
            return;
        if (httpContext.IsLocal())
        {
            context.Succeed(requirement);
        }
    }
}
