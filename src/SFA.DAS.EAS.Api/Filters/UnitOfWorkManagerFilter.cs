using System.Web.Http.Filters;
using SFA.DAS.EAS.Infrastructure.Data;
using WebApi.StructureMap;

namespace SFA.DAS.EAS.Account.Api.Filters
{
    public class UnitOfWorkManagerFilter : ActionFilterAttribute
    {
        public override void OnActionExecuted(HttpActionExecutedContext actionExecutedContext)
        {
            if (actionExecutedContext.Exception == null)
            {
                actionExecutedContext.GetService<IUnitOfWorkManager>().End();
            }
        }
    }
}