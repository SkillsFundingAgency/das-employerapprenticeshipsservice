using System.Web.Http.Controllers;
using System.Web.Http.Filters;
using SFA.DAS.EAS.Infrastructure.Data;
using WebApi.StructureMap;

namespace SFA.DAS.EAS.Account.Api.Filters
{
    public class UnitOfWorkManagerFilter : ActionFilterAttribute
    {
        public override void OnActionExecuting(HttpActionContext actionContext)
        {
            actionContext.Request.GetService<IUnitOfWorkManager>().Begin();
        }

        public override void OnActionExecuted(HttpActionExecutedContext actionExecutedContext)
        {
            actionExecutedContext.Request.GetService<IUnitOfWorkManager>().End(actionExecutedContext.Exception);
        }
    }
}