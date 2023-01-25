using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using SFA.DAS.CommitmentsV2.Api.Types.Http;
using SFA.DAS.EmployerAccounts.Api.Extensions;
using SFA.DAS.EmployerAccounts.Api.Http;

namespace SFA.DAS.EmployerAccounts.Api.Filters;

public class ValidateModelStateFilter : ActionFilterAttribute
{
    public override void OnActionExecuting(ActionExecutingContext context)
    {
        if (!context.ModelState.IsValid)
        {
            context.HttpContext.Response.SetSubStatusCode(HttpSubStatusCode.DomainException);
            context.Result = new BadRequestObjectResult(context.ModelState.CreateErrorResponse());
        }
    }
}